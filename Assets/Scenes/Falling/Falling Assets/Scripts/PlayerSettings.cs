using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
{
    public Rigidbody playerRb;
    public ParticleSystem fallingParticles;
    private bool isPlaying = false;
    FallingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
    }
    void Start()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        if (fallingParticles != null)
        {
            fallingParticles.Stop();
        }
    }

    void Update()
    {
        float currentVelocityY = playerRb.velocity.y;

        // Fast falling state
        if (currentVelocityY < -4f)
        {
            if (!isPlaying)
            {
                fallingParticles.Play();
                isPlaying = true;
                audioManager.playLongSFX(audioManager.fallingFast);
            }
        }
        // Umbrella/parachute state (approximately -2)
        else if (currentVelocityY > -4f && currentVelocityY < -1f)
        {
            if (isPlaying) 
            {
                fallingParticles.Stop();
                isPlaying = false;
            }
            audioManager.playLongSFX(audioManager.fallingSlow);
        }
        // Not falling significantly 
        else if (isPlaying)
        {
            fallingParticles.Stop();
            audioManager.stopLongSFX();
        }
    }
}
