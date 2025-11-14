using UnityEngine;
using UnityEngine.XR;

public class Reins : MonoBehaviour
{
    [Header("Positions")]
    public Transform left;    // Creature/Object left part
    public Transform right;   // Creature/Object right part
    public Transform leftHand;  // XR Left Hand
    public Transform rightHand; // XR Right Hand

    [Header("Reins")]
    private LineRenderer line;
    public Material glowingMaterial;

    void Start()
    {
        line = this.gameObject.GetComponent<LineRenderer>();

        if (line == null)
        {
            Debug.LogError("LineRenderer component missing! Please add a LineRenderer to this GameObject.");
            return;
        }

        line.positionCount = 4; // 2 reins (Left -> Left hand, Right -> Right hand)
        line.startWidth = 0.05f; 
        line.endWidth = 0.02f;

        if (glowingMaterial != null)
        {
            line.material = glowingMaterial;
        }
    }

    void Update()
    {
        line.SetPosition(0, left.position);
        line.SetPosition(1, leftHand.position);
        line.SetPosition(2, right.position);
        line.SetPosition(3, rightHand.position);
    }
}
