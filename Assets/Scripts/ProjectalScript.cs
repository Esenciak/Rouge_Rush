using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

public class ProjectalScript : MonoBehaviour
{
    public float projSpeed = 2f;
    private float timer = 0f;
    private float timeReset = 1f;
    public GameObject target;
    public Rigidbody2D rigBody;


    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Enemy");
        rigBody = GetComponent<Rigidbody2D>();
        rigBody.velocity = transform.position * projSpeed *Time.deltaTime;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (timer < timeReset)
        {
            //Debug.Log("timer++");
            timer = timer + Time.deltaTime;
            //transform.position = Vector2.MoveTowards(transform.position, target.transform.position, projSpeed * Time.deltaTime);

        }
        else
        {
            Debug.Log("time is up, fireball was destroyed!!");
            timeReset = 0;
            Destroy(gameObject);
           // Destroy(rigBody);
        }
    }

    private void nTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
