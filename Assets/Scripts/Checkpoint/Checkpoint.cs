using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the checkpoint trigger: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the checkpoint trigger!");

            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("PlayerController found, calling SaveGamePrepare and Save.");
                player.SaveGamePrepare();
                LoadSaveManager.Instance.Save("Checkpoint1.xml");
                Debug.Log("Checkpoint reached and game saved!");
            }
            else
            {
                Debug.LogWarning("PlayerController not found on Player object!");
            }
        }
    }
}