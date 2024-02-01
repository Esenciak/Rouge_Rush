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
[RequireComponent(typeof(FireWeaponEvent))]
[RequireComponent(typeof(FireWeapon))]
[RequireComponent(typeof(WeaponFiredEvent))]
[RequireComponent(typeof(AimWeaponEvent))]
[RequireComponent(typeof(AimWeapon))]
[RequireComponent(typeof(MovementByVelocityEvent))]
[RequireComponent(typeof(MovementByVelocity))]
[RequireComponent(typeof(SetActiveWeaponEvent))]
[RequireComponent(typeof(ActiveWeapon))]

#endregion WYMAGANE COMPONENTY

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailsSO playerDetails;
	[HideInInspector] public Health health;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;
	[HideInInspector] public IdleEvent idleEvent;
	[HideInInspector] public AimWeaponEvent aimWeaponEvent;
	[HideInInspector] public MovementByVelocityEvent movementByVelocityEvent;
	[HideInInspector] public SetActiveWeaponEvent setActiveWeaponEvent;
	[HideInInspector] public ActiveWeapon activeWeapon;
	[HideInInspector] public FireWeaponEvent fireWeaponEvent;
	[HideInInspector] public WeaponFiredEvent weaponFiredEvent;

	public List<Weapon> weaponList = new List<Weapon>();



	private void Awake()
	{
		
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		idleEvent = GetComponent<IdleEvent>();
		aimWeaponEvent = GetComponent<AimWeaponEvent>();
		movementByVelocityEvent = GetComponent<MovementByVelocityEvent>();
		setActiveWeaponEvent = GetComponent<SetActiveWeaponEvent>();
		activeWeapon = GetComponent<ActiveWeapon>();
		fireWeaponEvent = GetComponent<FireWeaponEvent>();
		weaponFiredEvent = GetComponent<WeaponFiredEvent>();
	}

	public void Initialize(PlayerDetailsSO playerDetails)
	{
		this.playerDetails = playerDetails;

		SetPlayerHealth();
		CreatePlayerStartingWeapons();
	}

	private void SetPlayerHealth()
	{
		health.SetStartingHealth(playerDetails.playerHealthAmount);
	}

	private void CreatePlayerStartingWeapons()
	{
		
		weaponList.Clear();

		
		foreach (WeaponDetailsSO weaponDetails in playerDetails.startingWeaponList)
		{
			
			AddWeaponToPlayer(weaponDetails);
		}
	}

	public Weapon AddWeaponToPlayer(WeaponDetailsSO weaponDetails)
	{
		Weapon weapon = new Weapon() { weaponDetails = weaponDetails, weaponReloadTimer = 0f, weaponClipRemainingAmmo = weaponDetails.weaponClipAmmoCapacity, weaponRemainingAmmo = weaponDetails.weaponAmmoCapacity, isWeaponReloading = false };

		weaponList.Add(weapon);
		weapon.weaponListPosition = weaponList.Count;		
		setActiveWeaponEvent.CallSetActiveWeaponEvent(weapon);

		return weapon;

	}
}
 