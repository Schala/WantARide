using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

/// <summary>
/// Disposes of entities marked for deletion
/// </summary>
[AlwaysSynchronizeSystem]
public class DeleteSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var buffer = new EntityCommandBuffer(Allocator.TempJob);
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref DeleteTag delete) =>
        {
            if (delete.lifetime >= delete.lifetimeMax)
                buffer.DestroyEntity(entity);
            delete.lifetime += deltaTime;
        }).Schedule();

        buffer.Playback(EntityManager);
        buffer.Dispose();
    }
}
