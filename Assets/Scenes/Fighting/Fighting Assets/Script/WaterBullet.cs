using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaterBullet : MonoBehaviour
{

    public GameObject explosionPrefab; // Prefab to instantiate upon collision
    public TrailRenderer trail;

    void Start()
    {
      
    }



    void OnCollisionEnter(Collision other)
    {
        // Instantiate the explosion prefab at the current water bullet's position and rotation
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        //trail.transform.parent = explosion.transform;
        trail.autodestruct = true;
        Destroy(gameObject, 0.1f); 
        Destroy(explosion, 0.1f); 
    }

    void Update()
    {
        
    }
}
