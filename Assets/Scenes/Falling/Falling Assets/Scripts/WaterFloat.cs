using UnityEngine;

public class WaterFloat : MonoBehaviour
{
    public float floatHeight = 5f;  
    public float bounceDamp = 2.0f;   
    public float waterDrag = 2.5f;    
    public float buoyancyForce = 10f;

    private Rigidbody rb;

    FallingAudioManager audioManager;
    private bool isPlayed = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isPlayed) // Play audio only once
        {
            audioManager.playSFX(audioManager.water);
            isPlayed = true;
        }
        
    }
    void OnTriggerStay(Collider other)
    {
        audioManager.stopLongSFX(); // Stop falling SFX

        if (other.CompareTag("Water"))  
        {
            float waterLevel = other.transform.position.y;  // Water height
            float difference = waterLevel - transform.position.y + floatHeight;

            if (difference > 0) // If character is below the water surface
            {
                // Add an upward force proportional to depth
                float force = difference * buoyancyForce - rb.velocity.y * bounceDamp;
                rb.AddForce(Vector3.up * force, ForceMode.Acceleration);
                rb.drag = waterDrag;  // Add water resistance
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            rb.drag = 0;  // Reset drag when leaving water
        }
    }
}
