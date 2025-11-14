using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using TMPro;

public class CreatureController : MonoBehaviour
{
    [Header("Initial Positions")]
    private Vector3 initialLeftHandPos;
    private Vector3 initialRightHandPos;
    private Quaternion initialCreatureRotation;
    private Vector3 initialCreaturePosition;

    [Header("Movement")]
    private float speed = 7f;
    private float moveSpeed = 3f;
    private float MoveThreshold = 0.1f;
    private bool canMove = true;

    [Header("Smoothing")]
    private float positionSmoothTime = 0.2f;
    private float rotationSmoothTime = 0.2f;
    private float inputSmoothingFactor = 0.1f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 smoothedLeftPos;
    private Vector3 smoothedRightPos;

    [Header("Instructions")]
    public TextMeshProUGUI instructionText;
    public float tutorialCompletionTime = 1.5f;
    private float tutorialTimer = 0f;
    public GameObject creatureControlHint;

    FallingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FallingAudioManager>();
    }

    void Start()
    {
        initialCreatureRotation = transform.rotation;
        initialCreaturePosition = transform.position;
        GetInitialHandCoordinates();

        instructionText.text = instructions[(int)currentState];
        creatureControlHint.SetActive(true);
    }

    void Update()
    {
        if (canMove)
        {
            HandleReinsAcceleration();
        }
        HandleReinsSteering();
        HandleTutorialProgress();
    }

    private enum TutorialState
    {
        RightHandRight,
        LeftHandLeft,
        BothHandsUp,
        BothHandsDown,
        BothHandsForward,
        BothHandsBack,
        Completed
    }
    private TutorialState currentState = TutorialState.RightHandRight;

    private readonly string[] instructions = {
        "Move RIGHT controller to the RIGHT to turn right",
        "Move LEFT controller to the LEFT to turn left",
        "Raise BOTH controllers UP to dive down",
        "Lower BOTH controllers DOWN to rise up",
        "Push BOTH controllers FORWARD to go faster",
        "Pull BOTH controllers BACK to slow down",
        "Tutorial completed! Enjoy your flight!"
    };

    void GetInitialHandCoordinates()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandWorldPos) &&
            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandWorldPos))
        {
            // Convert world hand positions to the initial dragon's reference frame
            initialLeftHandPos = Quaternion.Inverse(initialCreatureRotation) * (leftHandWorldPos - transform.position);
            initialRightHandPos = Quaternion.Inverse(initialCreatureRotation) * (rightHandWorldPos - transform.position);
        }
    }

    void HandleReinsSteering()
    {
        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandWorldPos) &&
            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandWorldPos))
        {
            // Smooth input positions
            smoothedLeftPos = Vector3.Lerp(smoothedLeftPos, leftHandWorldPos, inputSmoothingFactor);
            smoothedRightPos = Vector3.Lerp(smoothedRightPos, rightHandWorldPos, inputSmoothingFactor);

            // Convert current world hand positions to the initial dragon's reference frame
            Vector3 leftHandRelative = Quaternion.Inverse(initialCreatureRotation) * (smoothedLeftPos - initialCreaturePosition);
            Vector3 rightHandRelative = Quaternion.Inverse(initialCreatureRotation) * (smoothedRightPos - initialCreaturePosition);

            // Compare hand positions to initial positions (left and right)
            float leftXOffset = leftHandRelative.x - initialLeftHandPos.x;
            float rightXOffset = rightHandRelative.x - initialRightHandPos.x;
            float leftYOffset = leftHandRelative.y - initialLeftHandPos.y;
            float rightYOffset = rightHandRelative.y - initialRightHandPos.y;
            float leftZOffset = leftHandRelative.z - initialLeftHandPos.z;
            float rightZOffset = rightHandRelative.z - initialRightHandPos.z;

            // Debug
            Debug.Log($"Left X Offset: {leftXOffset:F2}\n" +
            $"Right X Offset: {rightXOffset:F2}\n" +
            $"Left Y Offset: {leftYOffset:F2}\n" +
            $"Right Y Offset: {rightYOffset:F2}\n" +
            $"Left Z Offset: {leftZOffset:F2}\n" +
            $"Right Z Offset: {rightZOffset:F2}\n" +
            $"Speed: {speed:F2}\n");

            //Rotation Left and Right - now with smoother, proportional control
            float rotationAmount = 2f;
            float targetXRotation = transform.eulerAngles.y;
            if (leftXOffset < -MoveThreshold)
            {
                targetXRotation -= moveSpeed * rotationAmount;  // Rotate left proportionally
            }
            else if (rightXOffset > MoveThreshold)
            {
                targetXRotation += moveSpeed * rotationAmount;  // Rotate right proportionally
            }

            // Smoother rotation with dynamic speed based on input magnitude
            float rotationSpeed = Mathf.Clamp(Mathf.Abs(leftXOffset) + Mathf.Abs(rightXOffset), 1f, 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                Quaternion.Euler(0, targetXRotation, 0),
                                                Time.deltaTime * rotationSpeed);

            // Up/Down Movement (Y-axis) with smoother tilting
            if (canMove)
            {
                if (Mathf.Abs(leftYOffset) > MoveThreshold && Mathf.Abs(rightYOffset) > MoveThreshold)
                {
                    float tiltAmount = 10f; // Adjust this for more or less tilting
                    float tiltFactor = Mathf.Clamp01((Mathf.Abs(leftYOffset) + Mathf.Abs(rightYOffset)) / 0.2f);
                    Quaternion targetYRotation = transform.rotation;

                    if (leftYOffset > 0 && rightYOffset > 0) // Diving down
                    {
                        transform.position += Vector3.down * moveSpeed * Time.deltaTime;
                        targetYRotation = Quaternion.Euler(tiltAmount * tiltFactor,
                                                         transform.eulerAngles.y,
                                                         transform.eulerAngles.z);
                    }
                    else if (leftYOffset < 0 && rightYOffset < 0) // Rising up
                    {
                        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
                        targetYRotation = Quaternion.Euler(-tiltAmount * tiltFactor,
                                                          transform.eulerAngles.y,
                                                          transform.eulerAngles.z);
                    }

                    // Smoothly interpolate rotation for a natural movement effect
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetYRotation, Time.deltaTime * 2f);
                }
            }

            //Slow down/Speed up (Z-axis) - now with gradual speed changes
            if (Mathf.Abs(leftZOffset) > MoveThreshold && Mathf.Abs(rightZOffset) > MoveThreshold)
            {
                if (leftZOffset > 0 && rightZOffset > 0)
                {
                    speed = Mathf.Lerp(speed, 10f, Time.deltaTime * 3f);
                }
                else if (leftZOffset < 0 && rightZOffset < 0)
                {
                    speed = Mathf.Lerp(speed, 1f, Time.deltaTime * 3f);
                }
            }
            else
            {
                // Return to base speed when no input
                speed = Mathf.Lerp(speed, 2f, Time.deltaTime * 2f);
            }
        }
    }

    void HandleReinsAcceleration()
    {
        // Smoother forward movement using SmoothDamp
        Vector3 targetPosition = transform.position + transform.forward * speed * Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, positionSmoothTime);
        
        audioManager.playLongSFX(audioManager.creature);
    }

    void HandleTutorialProgress()
    {
        if (currentState == TutorialState.Completed || instructionText == null)
            return;

        bool actionCompleted = false;
        float leftXOffset = 0f, rightXOffset = 0f, leftYOffset = 0f, rightYOffset = 0f, leftZOffset = 0f, rightZOffset = 0f;

        InputDevice leftController = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        InputDevice rightController = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);

        if (leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftHandWorldPos) &&
            rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightHandWorldPos))
        {
            Vector3 leftHandRelative = Quaternion.Inverse(initialCreatureRotation) * (leftHandWorldPos - initialCreaturePosition);
            Vector3 rightHandRelative = Quaternion.Inverse(initialCreatureRotation) * (rightHandWorldPos - initialCreaturePosition);

            leftXOffset = leftHandRelative.x - initialLeftHandPos.x;
            rightXOffset = rightHandRelative.x - initialRightHandPos.x;
            leftYOffset = leftHandRelative.y - initialLeftHandPos.y;
            rightYOffset = rightHandRelative.y - initialRightHandPos.y;
            leftZOffset = leftHandRelative.z - initialLeftHandPos.z;
            rightZOffset = rightHandRelative.z - initialRightHandPos.z;
        }

        // Check current tutorial step requirements
        switch (currentState)
        {
            case TutorialState.RightHandRight:
                actionCompleted = rightXOffset > MoveThreshold;
                break;

            case TutorialState.LeftHandLeft:
                actionCompleted = leftXOffset < -MoveThreshold;
                break;

            case TutorialState.BothHandsUp:
                actionCompleted = (leftYOffset > MoveThreshold) && (rightYOffset > MoveThreshold);
                break;

            case TutorialState.BothHandsDown:
                actionCompleted = (leftYOffset < -MoveThreshold) && (rightYOffset < -MoveThreshold);
                break;

            case TutorialState.BothHandsForward:
                actionCompleted = (leftZOffset > MoveThreshold) && (rightZOffset > MoveThreshold);
                break;

            case TutorialState.BothHandsBack:
                actionCompleted = (leftZOffset < -MoveThreshold) && (rightZOffset < -MoveThreshold);
                break;
        }

        if (actionCompleted)
        {
            tutorialTimer += Time.deltaTime;
            instructionText.text = $"{instructions[(int)currentState]}\n(Keep holding for {tutorialCompletionTime - tutorialTimer:F1}s)";

            if (tutorialTimer >= tutorialCompletionTime)
            {
                AdvanceTutorial();
            }
        }
        else
        {
            tutorialTimer = 0f;
            instructionText.text = instructions[(int)currentState];
        }
    }

    void AdvanceTutorial()
    {
        currentState++;
        tutorialTimer = 0f;

        if (currentState != TutorialState.Completed)
        {
            instructionText.text = instructions[(int)currentState];
        }
        else //Completed tutorial
        {
            instructionText.text = instructions[(int)currentState];
            StartCoroutine(HideTutorialAfterDelay(3f));
        }
    }

    IEnumerator HideTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        creatureControlHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            // canMove checks if creatures can move forward
            canMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Barrier"))
        {
            canMove = true;
        }
    }
}