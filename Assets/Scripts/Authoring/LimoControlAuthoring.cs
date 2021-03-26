using Unity.Entities;
using UnityEngine;

public class LimoControlAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	[SerializeField] float forwardSpeed = 7f;
	[SerializeField] float turnSpeed = 5f;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var limoControl = new LimoControl
		{
			forwardSpeed = forwardSpeed,
			turnSpeed = turnSpeed
		};

		dstManager.AddComponentData(entity, limoControl);
		dstManager.AddComponent<LimoControlInput>(entity);
	}
}
