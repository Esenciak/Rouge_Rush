using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
	public Rigidbody2D myRididbody;
	[SerializeField] public float movementSpeed = 15.5f;
	public Vector2 movementDirection;

	void Start()
	{
		myRididbody = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
	{
		movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
	}

	void FixedUpdate()
	{
		myRididbody.velocity = movementDirection * movementSpeed*Time.deltaTime;
	}
}
