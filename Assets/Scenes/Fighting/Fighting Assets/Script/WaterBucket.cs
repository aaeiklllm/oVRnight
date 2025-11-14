using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterBucket : MonoBehaviour
{
    public GameObject waterBalloon;
    public Transform spawnPoint;
    GameObject currentBalloon;
    void Start()
    {
        
    }

    void Update()
    {
        if (currentBalloon == null)
        {
            SpawnWaterBalloon();  
        }   
    }

    public void SpawnWaterBalloon()
    {
        currentBalloon = Instantiate(waterBalloon, spawnPoint.position, spawnPoint.rotation);

        Rigidbody rb = currentBalloon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.WakeUp();
        }
    }
}
