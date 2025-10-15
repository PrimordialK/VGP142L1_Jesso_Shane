using UnityEngine;

public class VictoryPoint : MonoBehaviour
{
    public VictoryMenu victoryMenu; // Assign in Inspector

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger entered by: " + other.name);
        // Check if the object entering is the player
        if (other.CompareTag("Player"))
        {
            if (victoryMenu != null)
            {
                victoryMenu.ShowMenu();
            }
        }
    }
}
