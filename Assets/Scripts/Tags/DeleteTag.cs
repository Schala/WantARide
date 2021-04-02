using System;
using Unity.Entities;

/// Associated with entities marked for deletion
[Serializable]
public struct DeleteTag : IComponentData
{
	public float lifetimeMax;
	public float lifetime;
}
