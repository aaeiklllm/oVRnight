using UnityEngine;

public class Bouncy : MonoBehaviour
{
    public GameObject bouncyBlanket;
    public GameObject bouncyGround;
    private Collider blanketCollider;
    private Collider groundCollider;

    FallingAudioManager audioManager;
    private float lastSoundTime = 0f;
    private float soundCooldown = 0.2f;

    void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
        blanketCollider = bouncyBlanket.GetComponent<Collider>();
        groundCollider = bouncyGround.GetComponent<Collider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;

        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.thisCollider == blanketCollider &&
                layer == LayerMask.NameToLayer("Blanket") &&
                bouncyBlanket.activeInHierarchy)
            {
                PlaySound(audioManager.blanket);
                return;
            }

            if (contact.thisCollider == groundCollider &&
                layer == LayerMask.NameToLayer("City") &&
                bouncyGround.activeInHierarchy)
            {
                PlaySound(audioManager.elastic);
                return;
            }
        }
    }


    void PlaySound(AudioClip clip)
    {
        if (Time.time - lastSoundTime > soundCooldown)
        {
            audioManager.playSFX(clip);
            lastSoundTime = Time.time;
        }
    }
}
