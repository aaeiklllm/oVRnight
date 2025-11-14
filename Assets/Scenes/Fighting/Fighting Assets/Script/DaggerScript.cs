using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerScript : MonoBehaviour
{

    public GameObject explosionPrefab; // Prefab to instantiate upon collision
    public TrailRenderer trail;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        // Instantiate the explosion prefab at the current GameObject's position and rotation
        GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
        trail.transform.parent = explosion.transform;
        trail.autodestruct = true;
        Destroy(gameObject);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
