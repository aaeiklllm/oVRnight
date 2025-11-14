using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterBalloon : MonoBehaviour
{
    public GameObject explosionPrefab;
    public TrailRenderer trail;
    public bool balloonDestroyed = false;
    WaterBucket waterBucket;
    private Rigidbody rb;
    public float throwForce = 10f;
    FightingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnThrown(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            // Get controller's forward direction (no upward bias)
            Vector3 throwDirection = controllerInteractor.transform.forward;

            // Apply force straight forward
            rb.AddForce(throwDirection * 15f, ForceMode.Impulse);

            // Optional: Lock rotation to prevent curves
            rb.freezeRotation = true;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Default"))
        { 
            //Instantiate the explosion prefab at the current water balloon's position and rotation
            GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            trail.transform.parent = explosion.transform;
            trail.autodestruct = true;
            Destroy(gameObject, 0.1f); 
            Destroy(explosion, 0.1f);

            balloonDestroyed = true;
            audioManager.playSFX(audioManager.waterBalloon);

            Invoke(nameof(waterBucket.SpawnWaterBalloon), 0.2f);
        }
        
    }
}
