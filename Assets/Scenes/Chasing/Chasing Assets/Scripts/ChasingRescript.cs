using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChasingRescript : MonoBehaviour
{
    [Header("Pursuers")]
    public Pursuer pursuer;
    public List<GameObject> pursuers = new List<GameObject>();
    private GameObject currentActivePursuer;

    [Header("Pursuer Attributes")]

    [Header("Pursuer Behavior")]

    [Header("XR Origin")]
    public Transform xrOrigin;
    public Transform actionPosition;
    private bool hasMoved = false;

    public GameObject transformEffect;
    ChasingAudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<ChasingAudioManager>();
    }

    void Start()
    {
        if (pursuers.Count > 0)
        {
            foreach (GameObject pursuer in pursuers)
            {
                pursuer.SetActive(false); // Disable all
            }
            pursuers[0].SetActive(true); // Enable Man in Suit (first pursuer)
            currentActivePursuer = pursuers[0];
        }
    }

    public void SetPursuer(int index)
    {
        if (index >= 0 && index < pursuers.Count)
        {
            StartCoroutine(MoveXROriginToActionRoom());
            StartCoroutine(TransformPursuerAfterDelay(index));
        }
    }
    private IEnumerator TransformPursuerAfterDelay(int index)
    {
        yield return new WaitForSeconds(10f);

        Vector3 currentPosition = currentActivePursuer.transform.position;
        Quaternion currentRotation = currentActivePursuer.transform.rotation;

        currentActivePursuer.SetActive(false);

        Vector3 spawnPosition = currentPosition;
        GameObject effect = Instantiate(transformEffect, spawnPosition, Quaternion.identity);
        audioManager.playSFX(audioManager.transformEffect);
        Destroy(effect, 1f);

        pursuers[index].SetActive(true);
        pursuers[index].transform.position = currentPosition;
        pursuers[index].transform.rotation = currentRotation;

        currentActivePursuer = pursuers[index];
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
