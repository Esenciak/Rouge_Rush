using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovementScript : MonoBehaviour
{
    public float EnemySpeed = 3.0f;
    public GameObject enemyObj;
    private float timer = 0;
    public float spawnrate = 2;
    

    // Start is called before the first frame update
    void Start()
    {
                
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnrate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawnEnemy();
            timer = 0;
        }
    }
    void spawnEnemy()
    {
        Instantiate(enemyObj, transform.position, transform.rotation);
    }
}
