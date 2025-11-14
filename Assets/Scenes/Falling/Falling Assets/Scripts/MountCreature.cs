using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections; 

public class MountCreature : MonoBehaviour
{
    public Transform mountPoint;
    public GameObject playerRig;
    private CreatureController creatureController;
    public GameObject turnObject;
    public GameObject moveObject;

    // Disabling Movement
    private ActionBasedSnapTurnProvider snapTurnProvider;
    private ActionBasedContinuousTurnProvider continuousTurnProvider;
    private ActionBasedContinuousMoveProvider continuousMoveProvider;

    public GameObject creatureHintText;
    private bool creatureHintShown = false;

    void Start()
    {
        creatureController = GetComponent<CreatureController>();
        if (creatureController != null)
            creatureController.enabled = false; 

        if (moveObject != null)
            moveObject.TryGetComponent(out continuousMoveProvider);

        if (turnObject != null)
        {
            turnObject.TryGetComponent(out snapTurnProvider);
            turnObject.TryGetComponent(out continuousTurnProvider);
        }
        Mount();
    }

    void Mount()
    {
        if (!creatureHintShown)
        {
            creatureHintText.SetActive(true);
            creatureHintShown = true;
        }

        StartCoroutine(WaitForMountConfirmation());
    }

    private IEnumerator WaitForMountConfirmation()
    {
        // Wait for A button press
        bool buttonPressed = false;
        while (!buttonPressed)
        {
            if (InputDevices.GetDeviceAtXRNode(XRNode.RightHand)
                .TryGetFeatureValue(CommonUsages.primaryButton, out bool pressed) && pressed)
            {
                buttonPressed = true;
            }
            yield return null;
        }

        creatureHintText.SetActive(false);
        StartCoroutine(MountCoroutine());
    }

    private IEnumerator MountCoroutine()
    {
        // Disable player movement and turning
        if (continuousMoveProvider) continuousMoveProvider.enabled = false;
        if (snapTurnProvider) snapTurnProvider.enabled = false;
        if (continuousTurnProvider) continuousTurnProvider.enabled = false;

        yield return new WaitForFixedUpdate();

        // Enable creature controller
        if (creatureController) creatureController.enabled = true;

        playerRig.transform.SetParent(mountPoint, false);
        playerRig.transform.localPosition = Vector3.zero;
        playerRig.transform.localRotation = Quaternion.identity;
    }
}
