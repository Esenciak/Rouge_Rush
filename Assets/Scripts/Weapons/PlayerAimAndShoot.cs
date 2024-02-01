using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimAndShoot : MonoBehaviour
{
	[SerializeField] private GameObject gun;


	private Vector2 worldPosition;
	private Vector2 direction;

	private void Update()
	{
		HandleGunRotation();
	}


	private void HandleGunRotation()
	{
		//obrót broni
		//worldPosition = Camera.main.ScreenToWorldPoint();
	}
}
