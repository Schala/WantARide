using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

/// Spawns a random explosion variant at the player's position upon collision
public class RandomExplodeSystem : SystemBase
{
	BuildPhysicsWorld buildPhysicsWorld;
	StepPhysicsWorld stepPhysicsWorld;
	BeginFixedStepSimulationEntityCommandBufferSystem bufferSystem;

	/// Execute the explosion on a worker thread.
	struct ExplodeJob : ITriggerEventsJob
	{
		[ReadOnly] public ComponentDataFromEntity<Explosion> explosionGroup;
		public EntityCommandBuffer buffer;
		public Entity explosionPrefab;
		public Entity player;
		public Translation playerTranslation;
		public Rotation playerRotation;

		public void Execute(TriggerEvent triggerEvent)
		{
			var a = triggerEvent.EntityA;
			var b = triggerEvent.EntityB;

			if (player == a || player == b) Explode();
		}

		/// If one of the collision entities is the player, execute the explosion.
		void Explode()
		{
			var explosion = buffer.Instantiate(explosionPrefab);
			var explosionData = explosionGroup[explosionPrefab];

			var delete = new DeleteTag
			{
				lifetimeMax = explosionData.destroySpeed,
				lifetime = 0f
			};

			buffer.SetComponent(player, new Rotation
			{
				Value = playerRotation.Value
			});
			buffer.SetComponent(player, new Translation
			{
				Value = playerTranslation.Value
			});
			buffer.AddComponent(explosion, delete);
		}
	}

	/// Gather the needed references in order to schedule the worker thread.
	protected override void OnCreate()
	{
		buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
		stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
		bufferSystem = World.GetOrCreateSystem<BeginFixedStepSimulationEntityCommandBufferSystem>();
	}

	protected override void OnUpdate()
    {
		var playerQuery = GetEntityQuery(typeof(PlayerTag)); // get the query, not the entity, because we need components as well

		var job = new ExplodeJob
		{
			explosionGroup = GetComponentDataFromEntity<Explosion>(true),
			buffer = bufferSystem.CreateCommandBuffer(),
			explosionPrefab = GameManager.ExplosionPrefab,
			player = playerQuery.GetSingletonEntity(),
			playerTranslation = playerQuery.GetSingleton<Translation>(),
			playerRotation = playerQuery.GetSingleton<Rotation>()
		};

		var handle = job.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);
		handle.Complete();
    }
}
