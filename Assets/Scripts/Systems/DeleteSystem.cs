using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

/// Disposes of entities marked for deletion
[AlwaysSynchronizeSystem]
public class DeleteSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffer = new EntityCommandBuffer(Allocator.TempJob);
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref DeleteTag delete) =>
        {
            delete.lifetime += deltaTime;

            if (delete.lifetime >= delete.lifetimeMax)
                buffer.DestroyEntity(entity);
        }).Schedule();

        buffer.Playback(EntityManager);
        buffer.Dispose();
    }
}
