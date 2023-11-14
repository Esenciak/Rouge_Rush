using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectalScript : MonoBehaviour
{
    public float projSpeed;

    private Rigidbody2D rigBody;

    // Start is called before the first frame update
    void Start()
    {
        rigBody = GetComponent<Rigidbody2D>();
        rigBody.velocity = transform.position * projSpeed;
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
