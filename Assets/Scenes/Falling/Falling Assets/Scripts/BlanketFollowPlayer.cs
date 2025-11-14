using UnityEngine;

public class BlanketFollowPlayer : MonoBehaviour
{
    public Transform player; 
    public float followSpeed = 5f;

    void Update()
    {
        if (player != null)
        {
            // Get the target position
            //Player's X & Z then Blanket's Y
            Vector3 targetPosition = new Vector3(player.position.x, transform.position.y, player.position.z); 
            
            // Smoothly move the blanket towards the player's XZ position
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
    }
}
