using UnityEngine;

public class ClothDeform : MonoBehaviour
{
    public Cloth cloth;
    public Transform player;
    public float forceAmount = 5f;  // Adjust for better sinking effect
    public float forceRadius = 1f;  // Area around the player that gets affected

    void Update()
    {
        ApplyForceAtPlayerPosition();
    }

    void ApplyForceAtPlayerPosition()
    {
        if (cloth == null || player == null) return;

        Vector3 playerPosition = player.position;

        // Loop through each vertex in the Cloth to apply force
        foreach (var sphereCollider in cloth.sphereColliders)
        {
            if (sphereCollider.first != null)
            {
                Vector3 colliderPos = sphereCollider.first.transform.position;
                float distance = Vector3.Distance(playerPosition, colliderPos);

                if (distance < forceRadius)
                {
                    Vector3 forceDirection = (colliderPos - playerPosition).normalized;
                    cloth.externalAcceleration = forceDirection * forceAmount;
                }
            }
        }
    }
}
