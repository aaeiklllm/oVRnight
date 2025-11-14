using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class FallingRescript : MonoBehaviour
{
    [Header("Safe Landing")]
    public List<GameObject> safeLandings = new List<GameObject>();

    [Header("Creatures")]

    public List<GameObject> creatures = new List<GameObject>();

    [Header("Flight Devices")]
    public List<GameObject> flightDevices = new List<GameObject>();

    [Header("XR Origin")]
    public GameObject xrOrigin;
    public Transform actionPosition;
    public Transform creatureActionPosition;
    public CharacterController controller;
    public Rigidbody rb;
    private bool hasMoved = false;

    public GameObject RescriptingMenu;

    void Start()
    {
        controller.enabled = false;
        rb.useGravity = false;
        rb.isKinematic = false;

        if (safeLandings.Count > 0)
        {
            foreach (GameObject safeLanding in safeLandings)
            {
                safeLanding.SetActive(false); // Disable all
            }
        }

        if (creatures.Count > 0)
        {
            foreach (GameObject creature in creatures)
            {
                creature.SetActive(false); // Disable all
            }
        }

        if (flightDevices.Count > 0)
        {
            foreach (GameObject flightDevice in flightDevices)
            {
                flightDevice.SetActive(false); // Disable all
            }
        }
    }

    void Update()
    {
        
    }

    // ========== 1. Set Safe Landing ==========
    public void SetSafeLanding(int index)
    {
        rb.useGravity = true;

        if (index >= 0 && index < safeLandings.Count)
        {
            // Disable all safe landings
            if (index != 1)
            {
                foreach (GameObject safeLanding in safeLandings)
                {
                    safeLanding.SetActive(false);
                }
            }

            // Enable the selected safe landing
            StartCoroutine(MoveXROriginToActionRoom(actionPosition));
            safeLandings[index].SetActive(true);
        }
    }

    // ========== 2. Set Creature ==========
    public void SetCreature(int index)
    {
        controller.enabled = true;
        rb.isKinematic = true;

        if (index >= 0 && index < creatures.Count)
        {
            // Disable all creatures
            foreach (GameObject creature in creatures)
            {
                creature.SetActive(false);
            }

            // Enable the selected creature
            creatures[index].SetActive(true);
            StartCoroutine(MoveXROriginToActionRoom(creatureActionPosition));
        }
    }

    // ========== 3. Set Flight Device ==========
    public void SetFlightDevice(int index)
    {
        if (index >= 0 && index < flightDevices.Count)
        {
            // Disable all flight devices
            foreach (GameObject flightDevice in flightDevices)
            {
                flightDevice.SetActive(false);
            }
            
            // Enable the selected flight device
            flightDevices[index].SetActive(true);
            RescriptingMenu.SetActive(false);
            
        }
    }

    // ========== Move XR Origin to Action Position ==========
    private IEnumerator MoveXROriginToActionRoom(Transform targetPosition)
    {
        if (!hasMoved)
        {
            yield return new WaitForFixedUpdate(); // Wait for physics update
            xrOrigin.transform.SetParent(targetPosition, false);
            xrOrigin.transform.localPosition = Vector3.zero;
            xrOrigin.transform.localRotation = Quaternion.identity;
            hasMoved = true;
        }
    }


    //Allow to call from other methods
    public void CallMoveXROriginToActionRoom()
    {
        StartCoroutine(MoveXROriginToActionRoom(actionPosition));
    }
}
