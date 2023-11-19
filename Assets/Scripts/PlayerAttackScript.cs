using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform projPos;
    public GameObject projectile;
    public float speed = 2f;
    private float timer = 0;
    public float attackRate = 3;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //player input to attack
        if(Input.GetMouseButton(0)) 
        {
         
         shoot();
        }
       

    }
    void shoot()
    {
        // create projectile
        Instantiate(projectile, projPos.position, projPos.rotation);
        // kierowanie w strone
        Vector3 kierunek = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        projectile.GetComponent<Rigidbody2D>().velocity = new Vector2(kierunek.x, kierunek.y) * speed * 2f;

        // destroy projectile
        //Destroy(projectile, 2f);
    }
    
}
