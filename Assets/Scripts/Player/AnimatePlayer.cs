using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Player))]
[DisallowMultipleComponent]
public class AnimatePlayer : MonoBehaviour
{
	private Player player;

    private void Awake()
	{
		
		player = GetComponent<Player>();
	}
	
	private void OnEnable()
    {
		player.idleEvent.OnIdle += IdleEvent_OnIdle;
	}




	private void IdleEvent_OnIdle(IdleEvent idleEvent)
	{
		//InitializeRollAnimationParameters();
		SetIdleAnimationParameters();
	}

	private void AimWeaponEvent_OnWeaponAim(AimWeaponEvent aimWeaponEvent, AimWeaponEventArgs aimWeaponEventArgs)
	{
		//InitializeAimAnimationParameters();
		//InitializeRollAnimationParameters();
		//SetAimWeaponAnimationParameters(aimWeaponEventArgs.aimDirection);
	}

	private void SetIdleAnimationParameters()
	{
		player.animator.SetBool(Settings.isMoving, false);
		player.animator.SetBool(Settings.isIdle, true);
	}

	private void SetAimWeaponAnimationParameters(AimDirection aimDirection)
	{
		// Set aim direction
		switch (aimDirection)
		{
			case AimDirection.Up:
				player.animator.SetBool(Settings.aimUp, true);
				break;

			case AimDirection.Right:
				player.animator.SetBool(Settings.aimRight, true);
				break;

			case AimDirection.Left:
				player.animator.SetBool(Settings.aimLeft, true);
				break;

			case AimDirection.Down:
				player.animator.SetBool(Settings.aimDown, true);
				break;

		}

	}
}
