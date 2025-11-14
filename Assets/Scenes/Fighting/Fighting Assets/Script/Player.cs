using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UIElements;


public class Player : MonoBehaviour
{
        private Material[] originalMaterials; // Array to store original materials of the character

        [Header("Flash")]

        [Header("Powers")]
        public GameObject dagger;
        public ParticleSystem lightning;
        public ParticleSystem healFX;
        public ParticleSystem barrier;
        public Collider barrierCollider;
        public GameObject lightningHitbox;
        public GameObject punchHit;

    [Header("Abilities")]
        public float abilityCooldown = 1f; // Cooldown between abilities
        public int currentAbilityIndex = 0; // Index of the current selected ability
        public float lastAbilityTime = 0f; // Time of the last ability usage
        public int abilitiesUnlocked = 3;

        [Header("Attack/Block")]
        public bool isAttacking;
        public bool isBlocking = false;

        [Header("Extras")]
        public bool canReceiveInput = true;
        public bool inputReceived;
        private GameObject _mainCamera;

        [Header("Punch")]
        private bool isRightGripping = false;
        private bool isLeftGripping = false;
        public GameObject rightGlove;
        public GameObject leftGlove;    
        public ActionBasedController rightController;
        public ActionBasedController leftController;
        public InputActionProperty rightVelocityAction;
        public InputActionProperty leftVelocityAction;
        private float punchThreshold = 3f; 

        [Header("Block")]
        public InputActionProperty leftTriggerAction;

        [Header("Hit Overlay")]
        public UnityEngine.UI.Image hitOverlay;
        public float hitDuration = 0.05f;

        [Header("WaterGun")]
        public InputActionProperty shootAction;
        public WaterGun waterGun;
        private bool isWaterGunGrabbed;

        [Header("Instructions")]
        public GameObject punchHintText; 
        private bool punchHintShown = false;
        public GameObject blockHintText;
        private bool blockHintShown = false;

        FightingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }


    void Start()
    {
        hitOverlay.gameObject.SetActive(false);
        barrierCollider.enabled = false;
    }

    void Update()
    {
        ActivateBoxingGlove();
        Combat();
    
        isWaterGunGrabbed = waterGun.isGrabbed;

        if (isAttacking)
        {
            CheckContinuousHits();
        }
    }

   private bool CheckGrip(ActionBasedController controller)
    {
        if (controller.selectAction.action != null)
        {
            float gripValue = controller.selectAction.action.ReadValue<float>();
            return gripValue > 0.5f; 
        }
        return false;
    }

    private void ActivateBoxingGlove()
    {
        isRightGripping = CheckGrip(rightController);
        isLeftGripping = CheckGrip(leftController);

        //Debug.Log("Right IsHandEmpty: " + IsHandEmpty(rightController));
        //Debug.Log("Left IsHandEmpty: " + IsHandEmpty(leftController));


        bool rightEmpty = isRightGripping && IsHandEmpty(rightController);
        bool leftEmpty = isLeftGripping && IsHandEmpty(leftController);

        rightGlove.SetActive(rightEmpty);
        leftGlove.SetActive(leftEmpty);

        if ((rightEmpty || leftEmpty) && !punchHintShown)
        {
            punchHintText.SetActive(true);
        }
        else
        {
            punchHintText.SetActive(false);
        }
    }


    private void Combat() 
    {
        if (isWaterGunGrabbed && shootAction.action.WasPressedThisFrame())
        {
            waterGun.Shoot();
        }
        else
        {
            //Punch
            CheckPunch();
            
        }

        // Block
        if (leftTriggerAction.action.WasPressedThisFrame() && !isAttacking) // Left trigger pressed
        {
            isBlocking = true;
            barrier.Play();
            barrierCollider.enabled = true;
        }
        else if (leftTriggerAction.action.WasReleasedThisFrame()) // Left trigger released
        {
            isBlocking = false;
            barrier.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            barrierCollider.enabled = false;
        }
    }

   public void CheckPunch() 
    {
     
        if (rightVelocityAction.action != null)
        {
            Vector3 rightVelocity = rightVelocityAction.action.ReadValue<Vector3>();
         
            if (rightVelocity.magnitude > punchThreshold) 
            {
                if (isRightGripping && IsHandEmpty(rightController)) 
                {
                    Debug.Log("Right Punch! Speed: " + rightVelocity.magnitude.ToString("F2"));
                    isAttacking = true;
                    Debug.Log("isAttacking set to TRUE");

                    Vector3 spawnPosition = rightController.transform.position + rightController.transform.forward * 0.2f; 
                    GameObject effect = Instantiate(punchHit, spawnPosition, Quaternion.identity);
                    audioManager.playSFX(audioManager.punch);

                    Destroy(effect, 1f);

                    if (!punchHintShown)
                    {
                        punchHintShown = true;
                        punchHintText.SetActive(false);
                    }
                }
            }
        }

        if (leftVelocityAction.action != null)
        {
            Vector3 leftVelocity = leftVelocityAction.action.ReadValue<Vector3>();
            if (leftVelocity.magnitude > punchThreshold)
            {
                if (isLeftGripping && IsHandEmpty(leftController))
                {
                    Debug.Log("Left Punch! Speed: " + leftVelocity.magnitude.ToString("F2"));
                    isAttacking = true;
                    Debug.Log("isAttacking set to TRUE");

                    Vector3 spawnPosition = leftController.transform.position + leftController.transform.forward * 0.1f;
                    GameObject effect = Instantiate(punchHit, spawnPosition, Quaternion.identity);
                    audioManager.playSFX(audioManager.punch);

                    Destroy(effect, 1f);

                    if (!punchHintShown)
                    {
                        punchHintShown = true;
                        punchHintText.SetActive(false);
                    }
                }
            }
        }
   }

    public bool IsHandEmpty(ActionBasedController controller)
    {
        // Get all interactors on this controller
        XRBaseInteractor[] interactors = controller.GetComponentsInChildren<XRBaseInteractor>(true);

        foreach (XRBaseInteractor interactor in interactors)
        {
            // Check if interactor has a selection
            if (interactor.hasSelection && interactor.firstInteractableSelected != null)
            {
                // Check if the selected object has XRGrabInteractable component
                if (interactor.firstInteractableSelected.transform.GetComponent<XRGrabInteractable>() != null)
                {
                    // This hand is holding a grabbable object
                    return false;
                }
            }
        }

        // No grabbable objects found in any interactor
        return true;
    }

    public void takeDamage() 
    {
        if (!isBlocking)
        {

            StartCoroutine(HitFlash());
            if (!blockHintShown)
            {
                blockHintText.SetActive(true);
            }
        }
        else
        {
            if (!blockHintShown)
            {
                blockHintShown = true;
                blockHintText.SetActive(false);
            }
            audioManager.playSFX(audioManager.block);
        }
    }

    IEnumerator HitFlash()
    {
        hitOverlay.gameObject.SetActive(true);
        yield return new WaitForSeconds(hitDuration);
        hitOverlay.gameObject.SetActive(false);
    }

    void CheckContinuousHits()
    {
        Vector3 attackOrigin = isRightGripping ? rightController.transform.position
                                             : leftController.transform.position;

        Collider[] hits = Physics.OverlapSphere(attackOrigin, 3f, LayerMask.GetMask("Enemy"));

        foreach (var hit in hits)
        {
            hit.GetComponent<MentorHitboxScript>().mentor.TakeDamage();
            isAttacking = false; // Stop after first hit
            break;
        }
    }
}
