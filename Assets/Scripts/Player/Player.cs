using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


[DisallowMultipleComponent]

#region WYMAGANE COMPONENTY
[RequireComponent(typeof(SortingGroup))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]


#endregion WYMAGANE COMPONENTY

public class Player : MonoBehaviour
{
	[HideInInspector] public PlayerDetailsSO playerDetails;
	[HideInInspector] public Health health;
	[HideInInspector] public SpriteRenderer spriteRenderer;
	[HideInInspector] public Animator animator;

	private void Awake()
	{
		
		health = GetComponent<Health>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
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
 