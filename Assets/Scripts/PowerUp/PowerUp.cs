using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Store reference to player for use in OnDestroy
            playerController = other.GetComponent<PlayerController>();
            Destroy(gameObject);
            Debug.Log("PowerUp collected and destroyed by player.");
        }
    }

    private PlayerController playerController;

    private void OnDestroy()
    {
        if (playerController != null)
        {
            playerController.jumpHeight *= 5f;
            Debug.Log("Player jumpHeight multiplied by 5!");
        }
    }
}
