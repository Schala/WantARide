using System;
using Unity.Entities;

/// <summary>
/// Associated with entities marked for deletion
/// </summary>
[Serializable]
public struct DeleteTag : IComponentData
{
	public float lifetimeMax;
	public float lifetime;
}
