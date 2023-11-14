using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform projPos;
    public GameObject projectile;
    public float speed = 2f;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //player input to attack
        if(Input.GetMouseButton(0)) 
        {
            Instantiate(projectile, projPos.position, projPos.rotation);
            
        }
       

    }

    
}
