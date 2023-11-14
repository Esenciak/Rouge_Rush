using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlerScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody2D myRididbody;
    [SerializeField] public float movementSpeed = 1.5f;
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
        myRididbody.velocity = movementDirection * movementSpeed;
    }
}