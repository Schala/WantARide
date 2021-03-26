using System;
using Unity.Entities;

[Serializable]
public struct SpinZone : IComponentData
{
	/// so this trigger only triggers once
	public const byte Triggered = 1;

	/// used to tell SpinZoneSystem.OnUpdate to start the spin
	public const byte StartSpin = 2;

	public float degreesToSpin;
	public float spinSpeed;

	/// keeps track and tells when to stop
	public float amountSpun;

	public Entity player;
	public byte flags;
}
