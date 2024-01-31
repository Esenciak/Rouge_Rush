using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]

public class PlayerControl : MonoBehaviour
{
	#region Tooltip

	[Tooltip("MovementDetailsSO scriptable object containing movement details such as speed")]

	#endregion Tooltip

	[SerializeField] private Transform weaponShootPosition;

	private Player player;


	private void Awake()
	{
		player = GetComponent<Player>();

		//moveSpeed = movementDetails.GetMoveSpeed();
	}


	private void Udpate()
	{

		MovementInput();	// przyciski od movementu

		
		WeaponInput();		// od broni

	}


	private void MovementInput()
	{
		// Get movement input
		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		float verticalMovement = Input.GetAxisRaw("Vertical");
		bool rightMouseButtonDown = Input.GetMouseButtonDown(1);

		// Create a direction vector based on the input
		Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

		// Adjust distance for diagonal movement (pythagoras approximation)
		if (horizontalMovement != 0f && verticalMovement != 0f)
		{
			direction *= 0.7f;
		}

		// If there is movement either move or roll
		if (direction != Vector2.zero)
		{
			//if (!rightMouseButtonDown)
			//{
			//	// trigger movement event
			//	player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
			//}
			//// else player roll if not cooling down
			//else if (playerRollCooldownTimer <= 0f)
			//{
			//	PlayerRoll((Vector3)direction);
			//}

		}
		// else trigger idle event
		else
		{
			player.idleEvent.CallIdleEvent();
		}
	}




	private void WeaponInput()
	{
		Vector3 weaponDirection;
		float weaponAngleDegrees, playerAngleDegrees;
		AimDirection playerAimDirection;

		// Aim weapon input
		AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);

		// Fire weapon input
		//FireWeaponInput(weaponDirection, weaponAngleDegrees, playerAngleDegrees, playerAimDirection);

		// Switch weapon input
		//SwitchWeaponInput();

		// Reload weapon input
		//ReloadWeaponInput();
	}

	private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
    {
        // Get mouse world position
        Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();
		weaponDirection = (mouseWorldPosition - weaponShootPosition.position);
      //  weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());
        Vector3 playerDirection = (mouseWorldPosition - transform.position);
        weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
        playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);   
        playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
        player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);
    }

}
