using Unity.Entities;
using UnityEngine;

public class SpinZoneAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	[SerializeField] float degreesToSpin = 90f;
	[SerializeField] float spinSpeed = 70f;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var spinZone = new SpinZone
		{
			degreesToSpin = degreesToSpin,
			spinSpeed = spinSpeed,
			amountSpun = 0f,
			player = Entity.Null,
			flags = 0
		};

		dstManager.AddComponentData(entity, spinZone);
	}
}
