using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ProjectalScript : MonoBehaviour
{
    public float projSpeed = 2f;
    

    public Rigidbody2D rigBody;

    // Start is called before the first frame update
    void Start()
    {
        rigBody = GetComponent<Rigidbody2D>();
        rigBody.velocity = transform.position * projSpeed *Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void nTriggerEnter2D(Collider2D collision)
    {
        Destroy(collision.gameObject);
        Destroy(gameObject);
    }
}
