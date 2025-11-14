using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class WaterGun : MonoBehaviour
{
    public GameObject bullet;
    public Transform firePoint; //where bullets spawn
    public bool isGrabbed = false;
    public GameObject WaterGunHintText;
    private bool WaterGunHintShown = false;
    FightingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        WaterGunHintText.SetActive(true);
    }

    public void OnReleased(SelectExitEventArgs args)
    {
        isGrabbed = false;
    }

    public void Shoot()
    {
        WaterGunHintShown = true;
        WaterGunHintText.SetActive(false);

        GameObject projectile = Instantiate(bullet, firePoint.position, firePoint.rotation);
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.AddForce(projectile.transform.forward * 10f, ForceMode.Impulse);
        audioManager.playSFX(audioManager.waterGun);
    }
}
