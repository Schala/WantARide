using System;
using Unity.Entities;

[Serializable]
public struct Explosion : IComponentData
{
	public float destroySpeed;
}
