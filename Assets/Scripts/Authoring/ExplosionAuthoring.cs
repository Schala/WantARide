using Unity.Entities;
using UnityEngine;

public class ExplosionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	[SerializeField] float destroySpeed = 0.5f;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var explosion = new Explosion
		{
			destroySpeed = destroySpeed
		};

		dstManager.AddComponentData(entity, explosion);
	}
}
