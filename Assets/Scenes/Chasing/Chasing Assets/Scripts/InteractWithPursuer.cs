using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class InteractWithPursuer : MonoBehaviour
{
    public GameObject playerObject;
    public float interactionDistance = 3f;
    public GameObject Interaction;
    public GameObject DialogueBox;
    public DialogueManager dialogueManager;
    public Animator animator;
    public Pursuer pursuer;

    private bool hasInteracted = false;
    private bool initialDialogueCompleted = false;
    private bool isDialogueActive = false;
    private bool buttonPressed = false;
    private bool waitingForButtonRelease = false;

    void Start()
    {
        Interaction.SetActive(false);
        DialogueBox.SetActive(false);
    }

    void Update()
    {
        if (playerObject == null) return;

        float distance = Vector3.Distance(transform.position, playerObject.transform.position);

        // Check for B button press
        bool secondaryButtonPressed = false;
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButtonPressed);

        if (secondaryButtonPressed)
        {
            if (!waitingForButtonRelease)
            {
                buttonPressed = true;
                waitingForButtonRelease = true;
            }
        }
        else
        {
            waitingForButtonRelease = false;
        }

        // Initial interaction check
        if (distance <= interactionDistance && !hasInteracted)
        {
            if (!DialogueBox.activeSelf)
            {
                Interaction.SetActive(true);
            }

            if (buttonPressed)
            {
                buttonPressed = false;
                StartInitialInteraction();
            }
        }
        else if (!hasInteracted)
        {
            Interaction.SetActive(false);
        }

        // Dialogue continuation
        if (isDialogueActive && buttonPressed)
        {
            buttonPressed = false;
            ContinueDialogue();
        }
    }

    public void StartInitialInteraction()
    {
        hasInteracted = true;
        Interaction.SetActive(false);
        DialogueBox.SetActive(true);

        DialogueTrigger dialogueTrigger = GetComponent<DialogueTrigger>();
        dialogueTrigger.TriggerDialogue(true); // Trigger initial dialogue
        isDialogueActive = true;
    }

    public void ContinueDialogue()
    {
        dialogueManager.DisplayNextSentence();

        // Check if dialogue ended
        if (dialogueManager.sentences.Count == 0)
        {
            EndDialogue();

            // Start random conversations after initial dialogue is complete
            if (!initialDialogueCompleted)
            {
                initialDialogueCompleted = true;
                StartCoroutine(RandomConversations());
            }
        }
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        DialogueBox.SetActive(false);
    }

    IEnumerator RandomConversations()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            float distance = Vector3.Distance(transform.position, playerObject.transform.position);
            if (distance < interactionDistance && !isDialogueActive)
            {
                DialogueBox.SetActive(true);
                isDialogueActive = true;

                DialogueTrigger dialogueTrigger = GetComponent<DialogueTrigger>();
                dialogueTrigger.TriggerDialogue(false); // Trigger random dialogue
            }
        }
    }
}