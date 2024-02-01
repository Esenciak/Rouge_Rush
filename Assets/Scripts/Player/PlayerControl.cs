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

	[SerializeField] private MovementDetailsSO movementDetails;


	#region Tooltip

	[Tooltip("The player WeaponShootPosition gameobject in the hieracrchy")]

	#endregion Tooltip

	[SerializeField] private Transform weaponShootPosition;

	private Player player;
	private float moveSpeed;
	private int currentWeaponIndex = 1;

	private void Awake()
	{
		// Load components
		player = GetComponent<Player>();
		moveSpeed = movementDetails.GetMoveSpeed();
	}


	private void Start()
	{

		SetStartingWeapon();
	}

	private void Update()
	{

		// Process the player movement input
		MovementInput();

		// Process the player weapon input
		WeaponInput();

	}
	private void MovementInput()
	{
		player.idleEvent.CallIdleEvent();

		float horizontalMovement = Input.GetAxisRaw("Horizontal");
		float verticalMovement = Input.GetAxisRaw("Vertical");

		Vector2 direction = new Vector2(horizontalMovement, verticalMovement);

		if (horizontalMovement != 0f && verticalMovement != 0f)
		{
			direction *= 0.7f;
		}

		if (direction != Vector2.zero)
		{
			player.movementByVelocityEvent.CallMovementByVelocityEvent(direction, moveSpeed);
		}
		else
		{
			player.idleEvent.CallIdleEvent();
		}
	}

	private void SetStartingWeapon()
	{
		int index = 1;

		foreach (Weapon weapon in player.weaponList)
		{
			if (weapon.weaponDetails == player.playerDetails.startingWeapon)
			{
				SetWeaponByIndex(index);
				break;
			}
			index++;
		}
	}

	private void WeaponInput()
	{
		Vector3 weaponDirection;
		float weaponAngleDegrees, playerAngleDegrees;
		AimDirection playerAimDirection;


		AimWeaponInput(out weaponDirection, out weaponAngleDegrees, out playerAngleDegrees, out playerAimDirection);
	}

	private void AimWeaponInput(out Vector3 weaponDirection, out float weaponAngleDegrees, out float playerAngleDegrees, out AimDirection playerAimDirection)
	{


		Vector3 mouseWorldPosition = HelperUtilities.GetMouseWorldPosition();


		weaponDirection = (mouseWorldPosition - player.activeWeapon.GetShootPosition());

		Vector3 playerDirection = (mouseWorldPosition - transform.position);

		weaponAngleDegrees = HelperUtilities.GetAngleFromVector(weaponDirection);
		playerAngleDegrees = HelperUtilities.GetAngleFromVector(playerDirection);
		playerAimDirection = HelperUtilities.GetAimDirection(playerAngleDegrees);
		player.aimWeaponEvent.CallAimWeaponEvent(playerAimDirection, playerAngleDegrees, weaponAngleDegrees, weaponDirection);

	}

	private void SetWeaponByIndex(int weaponIndex)
	{
		if (weaponIndex - 1 < player.weaponList.Count)
		{
			currentWeaponIndex = weaponIndex;
			player.setActiveWeaponEvent.CallSetActiveWeaponEvent(player.weaponList[weaponIndex - 1]);
		}
	}


	#region Validation
#if UNITY_EDITOR
	private void OnValidate()
	{
		HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
	}
#endif

	#endregion Validation
}
