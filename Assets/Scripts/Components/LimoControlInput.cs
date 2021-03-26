using System;
using Unity.Entities;

[Serializable]
public struct LimoControlInput : IComponentData
{
	public float forwardMoveAmount;
	public float turnAmount;
}
