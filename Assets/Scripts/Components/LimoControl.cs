using System;
using Unity.Entities;

[Serializable]
public struct LimoControl : IComponentData
{
	public float forwardSpeed;
	public float turnSpeed;
}
