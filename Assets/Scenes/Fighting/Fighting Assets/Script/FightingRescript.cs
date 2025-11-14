using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightingRescript : MonoBehaviour
{
    [Header("Animation")]
    public Mentor mentor;
    public Animator animator;
    public RuntimeAnimatorController goofyController;
    public RuntimeAnimatorController attackingController;

    [Header("Aggressors")]
    public List<GameObject> aggressors = new List<GameObject>();
    private GameObject currentActiveAggressor;

    [Header("Weapon")]
    public List<GameObject> weapons = new List<GameObject>();
    public List<TrailRenderer> weaponTrails = new List<TrailRenderer>(); 
    private int currentWeaponIndex = 0;
    private bool isAttacking = false;

    [Header("XR Origin")]
    public Transform xrOrigin;
    public Transform actionPosition;
    private bool hasMoved = false;
    public PlayerHitboxScript playerHitboxScript;

    public GameObject transformEffect;
    FightingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<FightingAudioManager>();
    }



    void Start()
    {
         // Default aggressor is Mentor
        if (aggressors.Count > 0)
        {
            foreach (GameObject aggressor in aggressors)
            {
                aggressor.SetActive(false); // Disable all
            }
            aggressors[0].SetActive(true); // Enable Mentor (first aggressor)
            currentActiveAggressor = aggressors[0];
            playerHitboxScript.AssignMentor(Mentor.ActiveMentor);
        }

        // Default weapon is Sword
        if (weapons.Count > 0)
        {
            foreach (GameObject weapon in weapons)
            {
                weapon.SetActive(false); // Disable all
            }
            weapons[0].SetActive(true); // Enable first weapon (Sword)
            currentWeaponIndex = 0; // Track the active weapon index
        }

        // Default animator state is Attacking
        if (animator != null)
        {
            animator.runtimeAnimatorController = attackingController;
        }
    }

    void Update()
    {
        isAttacking = mentor.isAttacking;
        HandleWeaponTrail();
    }

    // ========== 1. Transform Aggressor into Comical Figure ==========
    public void SetAggressor(int index)
    {
        if (index >= 0 && index < aggressors.Count)
        {
            StartCoroutine(MoveXROriginToActionRoom());
            StartCoroutine(TransformAggressorWithDelay(index));
        }
    }
    private IEnumerator TransformAggressorWithDelay(int index)
    {
        yield return new WaitForSeconds(10f);

        Vector3 currentPosition = currentActiveAggressor.transform.position;
        Quaternion currentRotation = currentActiveAggressor.transform.rotation;

        currentActiveAggressor.SetActive(false);

        aggressors[index].SetActive(true);
        
        Vector3 spawnPosition = currentPosition;
        GameObject effect = Instantiate(transformEffect, spawnPosition, Quaternion.identity);
        audioManager.playSFX(audioManager.transformEffect);
        Destroy(effect, 1f);

        aggressors[index].transform.position = currentPosition;
        aggressors[index].transform.rotation = currentRotation;

        currentActiveAggressor = aggressors[index];
        playerHitboxScript.AssignMentor(Mentor.ActiveMentor);

        
    }

    // ========== 2. Replace Weapon with a Comical Object ==========
    public void SetWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            StartCoroutine(MoveXROriginToActionRoom());
            StartCoroutine(ActivateWeaponAfterDelay(index));
        }
    }

    IEnumerator ActivateWeaponAfterDelay(int index)
    {
        yield return new WaitForSeconds(10f);

        weapons[currentWeaponIndex].SetActive(false);

        Vector3 currentPosition = weapons[currentWeaponIndex].transform.position;

        Vector3 spawnPosition = currentPosition;
        GameObject effect = Instantiate(transformEffect, spawnPosition, Quaternion.identity);
        audioManager.playSFX(audioManager.transformEffect);
        Destroy(effect, 1f);

        weapons[index].SetActive(true);
        currentWeaponIndex = index;
    }

    private void HandleWeaponTrail()
    {
        // Disable all trails
        foreach (TrailRenderer trail in weaponTrails)
        {
            if (trail != null)
            {
                trail.emitting = false;
            }
        }

        // Enable the trail of the active weapon when attacking
        if (isAttacking && weaponTrails[currentWeaponIndex] != null)
        {
            weaponTrails[currentWeaponIndex].emitting = true;
        }
    }

    // ========== 3. Make Aggressor Perform Humorous Actions ==========
    public void MakeAggressorPerformAction(int actionIndex)
    {
        switch (actionIndex)
        {
            case 0:
                ShrinkAggressor();
                break;
            case 1:
                SlowMoAttack();
                break;
            case 2:
                Dance();
                break;
        }

        StartCoroutine(MoveXROriginToActionRoom());
    }

    void ShrinkAggressor()
    {
        StartCoroutine(MoveXROriginToActionRoom());
        StartCoroutine(ShrinkAfterDelay());
    }

    private IEnumerator ShrinkAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        animator.speed = 2.2f;
        Vector3 currentPosition = currentActiveAggressor.transform.position;
        Vector3 spawnPosition = currentPosition;
        GameObject effect = Instantiate(transformEffect, spawnPosition, Quaternion.identity);
        audioManager.playSFX(audioManager.transformEffect);
        Destroy(effect, 1f);

        currentActiveAggressor.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
    }


    void SlowMoAttack()
    {
        animator.speed = 0.4f;
    }

    void Dance()
    {
        audioManager.playLongSFX2(audioManager.dance);
        animator.runtimeAnimatorController = goofyController;
    }

    // ========== Move XR Origin to Action Position ==========
    private IEnumerator MoveXROriginToActionRoom()
    {
        if (!hasMoved)
        {
            yield return new WaitForFixedUpdate(); // Ensures physics updates before setting position
            xrOrigin.transform.SetParent(actionPosition, false);
            xrOrigin.transform.localPosition = Vector3.zero;
            xrOrigin.transform.localRotation = Quaternion.identity;
            hasMoved = true;
        }
    }
}
