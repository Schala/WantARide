using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class SpinZoneSystem : SystemBase
{
	BuildPhysicsWorld buildPhysicsWorld;
	StepPhysicsWorld stepPhysicsWorld;
	BeginFixedStepSimulationEntityCommandBufferSystem jobBufferSystem;
	Entity level = Entity.Null;
	Entity rotatable = Entity.Null;

	/// Execute the spinning on a worker thread.
	struct SpinJob : ITriggerEventsJob
	{
		public ComponentDataFromEntity<SpinZone> spinZoneGroup;
		public EntityCommandBuffer buffer;
		public Entity player;

		/// Identify our spin zone and our player.
		public void Execute(TriggerEvent triggerEvent)
		{
			var a = triggerEvent.EntityA;
			var b = triggerEvent.EntityB;

			var isSpinZoneA = spinZoneGroup.HasComponent(a);
			var isSpinZoneB = spinZoneGroup.HasComponent(b);

			if (isSpinZoneA && b == player) Spin(a);
			if (isSpinZoneB && a == player) Spin(b);
		}

		/// If one of the collision entities is the player, initiate spinning.
		void Spin(Entity spinZone)
		{
			var data = spinZoneGroup[spinZone];
			data.player = player;
			data.flags |= SpinZone.StartSpin | SpinZone.Triggered;
			buffer.SetComponent(spinZone, data);
		}
	}

	/// Gather the needed references in order to schedule the worker thread.
	protected override void OnCreate()
	{
		buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
		jobBufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
		level = GetSingletonEntity<PlayerTag>();
		rotatable = GetSingletonEntity<RotatableTag>();
	}

	/// Run our worker threads, and have the player spin if they're on any of the spin zones.
	protected override void OnUpdate()
    {
		var job = new SpinJob
		{
			spinZoneGroup = GetComponentDataFromEntity<SpinZone>(),
			buffer = jobBufferSystem.CreateCommandBuffer(),
			player = level
		};

		var handle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
		handle.Complete();

		var deltaTime = Time.DeltaTime;
		var rotationGroup = GetComponentDataFromEntity<Rotation>();
		var translationGroup = GetComponentDataFromEntity<Translation>();
		var buffer = new EntityCommandBuffer(Allocator.TempJob);
		var rotatable = this.rotatable;

		Entities.ForEach((ref SpinZone spinZone) =>
		{
			if ((spinZone.flags & SpinZone.StartSpin) != 0)
			{
				var playerRotation = rotationGroup[spinZone.player];
				var pivot = translationGroup[rotatable];
				var position = translationGroup[spinZone.player];

				// rotate around
				playerRotation.Value = quaternion.Euler(math.mul(quaternion.AxisAngle(math.up(), spinZone.spinSpeed * deltaTime), position.Value - pivot.Value) + pivot.Value);
				buffer.SetComponent(spinZone.player, playerRotation);

				spinZone.amountSpun += spinZone.spinSpeed * deltaTime;
			}
		}).Schedule();

		buffer.Playback(EntityManager);
		buffer.Dispose();
    }
}
