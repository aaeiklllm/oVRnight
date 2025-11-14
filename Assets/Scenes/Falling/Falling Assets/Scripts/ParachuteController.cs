using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class ParachuteController : MonoBehaviour
{
    [Header("Player")]
    public Transform playerBack; 
    public GameObject playerRig;
    public Rigidbody playerRb;
    public CharacterController characterController;

    [Header("Parachute Settings")]
    private Rigidbody rb;
    bool isWorn = false;
    bool frozen = false;
    private bool isDeployed = false;
    private bool canDeploy = false;
    public Animator parachuteAnimator;
    public GameObject parachuteModel;

    [Header("Instructions")]
    public GameObject parachuteHintText;
    private bool parachuteHintShown = false;

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
            transform.position = playerBack.position;
            transform.rotation = playerBack.rotation;

            if (!isDeployed)
            {
                StartCoroutine(SetCanDeploy(5f));
                CheckForDeployment();
            }
            else
            {
                HandleParachuteMovement();
            }
        }

        FreezeParachuteAnim();
    }

    public void AttachToPlayer()
    {
        rb.isKinematic = true; 
        rb.useGravity = false;

        rb.velocity = Vector3.zero; // Stop movement
        rb.angularVelocity = Vector3.zero; // Stop rotation

        transform.position = playerBack.position;
        transform.rotation = playerBack.rotation;

        isWorn = true;

        //Send Haptic Feedback
        InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).SendHapticImpulse(0, 0.5f, 0.2f);
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);
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
    private void HandleParachuteMovement()
    {
        if (playerRb.velocity.y < 0)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, -2f, playerRb.velocity.z);
        }
    }
    IEnumerator SetCanDeploy(float delay)
    {
        yield return new WaitForSeconds(delay);
        canDeploy = true;

        if (!parachuteHintShown)
        {
            parachuteHintText.SetActive(true);
            parachuteHintShown = true;
        }
    }

     private void CheckForDeployment()
    {
        if (canDeploy)
        {
            if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButton) && primaryButton)
            {
                parachuteHintText.SetActive(false);
                DeployParachute();
            }
        }
    }

    private void DeployParachute()
    {
        isDeployed = true;
        
        if (parachuteModel != null)
            parachuteModel.SetActive(true);
            
        if (parachuteAnimator != null)
            parachuteAnimator.SetTrigger("Deploy");
        
        // Send haptic feedback
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).SendHapticImpulse(0, 0.5f, 0.2f);
        audioManager.playSFX(audioManager.parachute);

    }   

    private void FreezeParachuteAnim()
    {
        if (!frozen && parachuteAnimator.GetCurrentAnimatorStateInfo(0).IsName("Deployed") &&
        parachuteAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
        {
            parachuteAnimator.speed = 0; // Freeze at last frame
            frozen = true;
        }
    }
}
