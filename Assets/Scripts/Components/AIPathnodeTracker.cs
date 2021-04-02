using System;
using Unity.Entities;
using Unity.Mathematics;

/// Original script by Devin Curry
[Serializable]
public struct AIPathnodeTracker : IComponentData
{
	public float moveSpeed;
	public float rotationSpeed;
	public int currNode;
	public float3 storedPos;
	public bool paused;
}
