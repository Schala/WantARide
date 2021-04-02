using Unity.Entities;
using Unity.Mathematics;

/// Original script by Devin Curry
public struct PathNode : IBufferElementData
{
	public float3 Value;

	public static implicit operator PathNode(float3 f3) => new PathNode { Value = f3 };
	public static implicit operator float3(PathNode pn) => pn.Value;
}
