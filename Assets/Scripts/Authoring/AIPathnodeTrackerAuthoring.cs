using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

/// Original script by Devin Curry
public class AIPathnodeTrackerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
	[SerializeField] float moveSpeed = 1f;
	[SerializeField] float rotationSpeed = 20f;

	public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
	{
		var pathnodeTracker = new AIPathnodeTracker
		{
			moveSpeed = moveSpeed,
			rotationSpeed = rotationSpeed,
			currNode = 0,
			storedPos = float3.zero,
			paused = false
		};

		dstManager.AddComponentData(entity, pathnodeTracker);
		dstManager.AddBuffer<PathNode>(entity);
	}
}
