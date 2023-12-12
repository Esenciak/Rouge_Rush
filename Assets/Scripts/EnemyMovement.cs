using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameObject target;

    private float timer = 0;
    public float timerReset = 2f;

    // Start is called before the first frame update
    void Start()
    {
       target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < timerReset)
        {
            timer = timer + Time.deltaTime;
            Movement();
            
        }
        else
        {
            //difMovement();
            Destroy(gameObject);
        }
        
    }


    void Movement()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
    }

    void difMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, target.transform.position, (moveSpeed * -1) * Time.deltaTime);

    }
}

 