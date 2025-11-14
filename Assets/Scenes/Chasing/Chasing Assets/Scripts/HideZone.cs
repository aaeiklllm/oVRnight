using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR;

public class HideZone : MonoBehaviour
{
    public GameObject hidePrompt; 
    public GameObject exitPrompt;
    private bool playerInZone = false;
    private bool isHiding = false;
    public XROrigin normalXROrigin; 
    public XROrigin hidingXROrigin;
    private bool wasButtonPressed = false;

    ChasingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<ChasingAudioManager>();
    }


    void Start()
    {
        normalXROrigin.gameObject.SetActive(true);
        hidingXROrigin.gameObject.SetActive(false);
    }

    void Update()
    {
        bool isButtonPressed = false;
        InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(CommonUsages.primaryButton, out isButtonPressed);

        if (playerInZone && isButtonPressed && !wasButtonPressed)
        {
            if (!isHiding)
            {
                EnterHide();
            }
            else
            {
                ExitHide();
            }
        }

        wasButtonPressed = isButtonPressed; 
    }


    private void EnterHide()
    {
        normalXROrigin.gameObject.SetActive(false);
        hidingXROrigin.gameObject.SetActive(true);

        isHiding = true;
        exitPrompt.SetActive(true);
        audioManager.playSFX(audioManager.hide);
        Pursuer.ActivePursuer.isPlayerHiding = true;
    }

    private void ExitHide()
    {
        normalXROrigin.gameObject.SetActive(true);
        hidingXROrigin.gameObject.SetActive(false);

        isHiding = false;
        exitPrompt.SetActive(false);
        audioManager.playSFX(audioManager.hide);
        Pursuer.ActivePursuer.isPlayerHiding = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            //player = other.transform;

            if (!isHiding)
            {
                hidePrompt.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            playerInZone = false;

            if (!isHiding)
            {
                hidePrompt.SetActive(false);
            }
        }
    }
}