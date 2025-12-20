using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.SaveGamePrepare();
                LoadSaveManager.Instance.Save("Checkpoint1.xml");
                Debug.Log("Checkpoint reached and game saved!");
            }
        }
    }
}