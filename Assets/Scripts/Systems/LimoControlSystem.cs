using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Movement and physics logic for player-controlled limos
/// </summary>
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class LimoControlSystem : SystemBase, InputActions.IPlayerActions
{
	InputActions inputActions = null;
	LimoControlInput input;

	public void OnFire(InputAction.CallbackContext context)
	{
		throw new System.NotImplementedException();
	}

	public void OnLook(InputAction.CallbackContext context)
	{
		throw new System.NotImplementedException();
	}

	/// <summary>
	/// Apply our forward and turn values.
	/// </summary>
	public void OnMove(InputAction.CallbackContext context)
	{
		var movement = context.ReadValue<Vector2>();
		input.forwardMoveAmount = movement.y;
		input.turnAmount = movement.x;
	}

	/// <summary>
	/// Get the player's limo control data.
	/// </summary>
	protected override void OnCreate()
	{
		inputActions = new InputActions();
		var playerQuery = GetEntityQuery(typeof(PlayerTag));
		input = playerQuery.GetSingleton<LimoControlInput>();
	}

	/// <summary>
	/// Apply force and rotation for the limo.
	/// </summary>
	protected override void OnUpdate()
	{
		var deltaTime = Time.DeltaTime;

		Entities.ForEach((ref Rotation rotation, ref PhysicsVelocity velocity, in LimoControl limoControl, in LimoControlInput input, in PhysicsMass mass) =>
		{
			PhysicsComponentExtensions.ApplyLinearImpulse(ref velocity, in mass, math.forward() * input.forwardMoveAmount * limoControl.forwardSpeed * deltaTime);
			rotation.Value = quaternion.RotateY(input.turnAmount * limoControl.turnSpeed * deltaTime);
		}).Run();
	}
}
