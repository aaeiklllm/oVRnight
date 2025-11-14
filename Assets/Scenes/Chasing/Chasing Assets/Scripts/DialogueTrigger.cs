using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue initialDialogue;
    public Dialogue randomDialogue;
    public DialogueManager dialogueManager;

    public void TriggerDialogue(bool isInitial = true)
    {
        if (isInitial)
        {
            Debug.Log("Part 1 Dialogue");
            dialogueManager.StartDialogue(initialDialogue);
            
        }
        else
        {
            Debug.Log("Part 2 Dialogue");
            dialogueManager.StartDialogue(randomDialogue);
            
        }
    }
}
