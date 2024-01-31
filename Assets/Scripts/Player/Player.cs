using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[DisallowMultipleComponent]

#region WYMAGANE COMPONENTY
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(PlayerControl))]
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AnimatePlayer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(IdleEvent))]
[RequireComponent(typeof(Idle))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]

#endregion WYMAGANE COMPONENTY

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailsSO playerDetails;
	[HideInInspector] public Health health;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;
	[HideInInspector] public IdleEvent idleEvent;
	[HideInInspector] public AimWeaponEvent aimWeaponEvent;

	private void Awake()
	{
		
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		idleEvent = GetComponent<IdleEvent>();
		aimWeaponEvent = GetComponent<AimWeaponEvent>();
	}

	public void Initialize(PlayerDetailsSO playerDetails)
	{
		this.playerDetails = playerDetails;

		SetPlayerHealth();
	}

	private void SetPlayerHealth()
	{
		health.SetStartingHealth(playerDetails.playerHealthAmount);
	}
}
 