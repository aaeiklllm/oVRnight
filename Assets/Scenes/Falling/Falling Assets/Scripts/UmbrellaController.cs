using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class UmbrellaController : MonoBehaviour
{
    [Header("Player")]
    public GameObject playerRig;
    public Rigidbody playerRb;
    private CharacterController characterController;

    [Header("Umbrella Settings")]
    public float glideFallSpeed = 2f;
    public Transform rightHandAttach;
    public Animator umbrellaAnimator;
    private Rigidbody rb;
    private bool isWorn = false;
    private bool isOpen = false;
    private bool buttonPressedLastFrame = false;

    [Header("Instructions")]
    public GameObject umbrellaHintText;
    private bool umbrellaHintShown = false;

    [Header("UI")]
    public FallingRescript fallingRescript;
    FallingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
    }


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerRb.useGravity = true;
    }

    void Update()
    {
        if (isWorn)
        {
            // Keep umbrella upright and attached to hand
            transform.position = rightHandAttach.position;
            transform.localRotation = Quaternion.Euler(0, 180, 0);


            HandleUmbrellaInput();
            HandleUmbrellaMovement();
        }
    }

    private void HandleUmbrellaInput()
    {
        // Check for primary button (A button on Oculus) press
        if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
        {
            if (!buttonPressedLastFrame)
            {
                ToggleUmbrella();
                buttonPressedLastFrame = true;
            }
        }
        else
        {
            buttonPressedLastFrame = false;
        }
    }

    public void AttachToPlayer()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = rightHandAttach.position;
        transform.localRotation = Quaternion.Euler(0, 180, 0);

        isWorn = true;
        
        //Send Haptic Feedback
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);

        if (!umbrellaHintShown)
        {
            umbrellaHintText.SetActive(true);
            umbrellaHintShown = true;
        }
    }

    public void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!isWorn)
        {
            AttachToPlayer();
            GetComponent<XRGrabInteractable>().enabled = false; // Prevent re-grabbing
            fallingRescript.CallMoveXROriginToActionRoom();
        }
    }

    private void HandleUmbrellaMovement()
    {
        if (isOpen)
        {
            if (playerRb.velocity.y < 0)
            {
                playerRb.velocity = new Vector3(playerRb.velocity.x, -2f, playerRb.velocity.z);
            }
        }
    }

    private void ToggleUmbrella()
    {
        umbrellaHintText.SetActive(false);

        isOpen = !isOpen;
        umbrellaAnimator.SetBool("isOpen", isOpen);
        audioManager.playSFX(audioManager.umbrella);

        // Send haptic feedback
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);

    }
}