using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class JetpackController : MonoBehaviour
{
    [Header("Player")]
    public Transform playerBack; 
    public GameObject playerRig; 
    public Rigidbody playerRb;
    public CharacterController characterController;
    public ContinuousMoveProviderBase moveProvider;

    [Header("Jetpack Settings")]
    public float jetpackForce = 15f;
    //public float moveSpeed = 3f;
    private float groundSpeed = 1f;
    public float flyingSpeed = 6f;
    private Rigidbody rb;
    public ParticleSystem jetPackParticleSystem;
    bool isWorn = false;

    [Header("Instructions")]
    public GameObject jetpackHintText;
    private bool jetpackHintShown = false;
    private float gripTimer;

    [Header("UI")]
    public FallingRescript fallingRescript;
    FallingAudioManager audioManager;
    private bool isJetpackSoundPlaying = false;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isWorn) 
        {
            transform.position = playerBack.position;
            transform.rotation = playerBack.rotation;

            HandleJetpackMovement();
        }
    }

    public void AttachToPlayer()
    {
        characterController.enabled = false;
        Destroy(characterController);

        // Enable physics on the player
        playerRb.useGravity = true;
        playerRb.isKinematic = false;

        // Disable physics on the jetpack
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = playerBack.position;
        transform.rotation = playerBack.rotation;

        isWorn = true;

        //Send Haptic Feedback
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0, 0.5f, 0.2f);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);

        if (!jetpackHintShown)
        {
            jetpackHintText.SetActive(true);
            jetpackHintShown = true;
        }
    }

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        if (transform.parent != playerBack) 
        {
            AttachToPlayer();
            GetComponent<XRGrabInteractable>().enabled = false; // Prevent re-grabbing
            fallingRescript.CallMoveXROriginToActionRoom();
        }
    }

    private void HandleJetpackMovement()
    {
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        bool isGripping = rightController.TryGetFeatureValue(CommonUsages.gripButton, out bool gripPressed) && gripPressed;

        Vector3 currentVelocity = playerRb.velocity;

        if (isGripping)
        {
            currentVelocity.y = jetpackForce;

            // Boost movement speed
            if (moveProvider) moveProvider.moveSpeed = flyingSpeed;

            // Effects
            if (!jetPackParticleSystem.isPlaying) jetPackParticleSystem.Play();
            if (!isJetpackSoundPlaying)
            {
                audioManager.playLongSFX2(audioManager.jetpack);
                isJetpackSoundPlaying = true;
            }

            // Hide hint
            if ((gripTimer += Time.deltaTime) > 2f)
                jetpackHintText.SetActive(false);
        }
        else 
        {
            if (moveProvider) moveProvider.moveSpeed = groundSpeed;

            // Stop effects
            if (jetPackParticleSystem.isPlaying) jetPackParticleSystem.Stop();
            if (isJetpackSoundPlaying)
            {
                audioManager.stopLongSFX2();
                isJetpackSoundPlaying = false;
            }
            gripTimer = 0f;
        }

        playerRb.velocity = currentVelocity;
    }
}
