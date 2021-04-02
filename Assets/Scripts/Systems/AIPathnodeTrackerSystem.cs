using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

/// Original script by Devin Curry
public class AIPathnodeTrackerSystem : SystemBase
{
	BuildPhysicsWorld buildPhysicsWorld;
	StepPhysicsWorld stepPhysicsWorld;
	BeginFixedStepSimulationEntityCommandBufferSystem jobBufferSystem;
	Entity player;

	/// Execute the collision logic on a worker thread.
	struct HitSomethingJob : ICollisionEventsJob
	{
		public ComponentDataFromEntity<Translation> translationGroup;
		[ReadOnly] public ComponentDataFromEntity<CollisionGroundTag> groundGroup;
		public ComponentDataFromEntity<AIPathnodeTracker> trackerGroup;
		public NativeArray<Entity> allNodes;
		public EntityCommandBuffer buffer;
		public Entity player;

		public void Execute(CollisionEvent collisionEvent)
		{
			var a = collisionEvent.EntityA;
			var b = collisionEvent.EntityB;

			var isPlayerA = a == player;
			var isPlayerB = b == player;

			var isNotGroundA = !groundGroup.HasComponent(a) && translationGroup.HasComponent(a);
			var isNotGroundB = !groundGroup.HasComponent(b) && translationGroup.HasComponent(b);

			if (isPlayerA && isNotGroundB) HitSomething(b);
			if (isPlayerB && isNotGroundA) HitSomething(a);
		}

		/// if the player collides with something other than the ground it will bounce back away from the wall a bit and clear all nodes
		void HitSomething(Entity thing)
		{
			var colPosition = translationGroup[thing];
			var playerPosition = translationGroup[player];
			var colVector = math.normalize(colPosition.Value - playerPosition.Value);

			var playerTracker = trackerGroup[player];
			buffer.SetComponent(player, new Translation
			{
				Value = playerTracker.storedPos
			});

			// Delete currently active PlayerPathNodes (as punishment for collision)
			// Then clear pathNodes list
			for (int i = 0; i < allNodes.Length; i++)
				buffer.AddComponent<DeleteTag>(allNodes[i]);

			playerTracker.currNode = 0;
			buffer.SetComponent(player, playerTracker);
		}
	}

	/// Gather the needed references in order to schedule the worker thread.
	protected override void OnCreate()
	{
		buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
		jobBufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
		player = GetSingletonEntity<PlayerTag>();
	}

	/// Run our worker threads, doing our node calculations.
	protected override void OnUpdate()
    {
		var nodeQuery = GetEntityQuery(typeof(PlayerPathNodeTag));
		var nodeEntities = nodeQuery.ToEntityArray(Allocator.Temp);

		var job = new HitSomethingJob
		{
			translationGroup = GetComponentDataFromEntity<Translation>(),
			groundGroup = GetComponentDataFromEntity<CollisionGroundTag>(true),
			trackerGroup = GetComponentDataFromEntity<AIPathnodeTracker>(),
			allNodes = nodeEntities,
			buffer = jobBufferSystem.CreateCommandBuffer(),
			player = player
		};

		var handle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
		handle.Complete();
		nodeEntities.Dispose();

		var deltaTime = Time.DeltaTime;
    }
}
