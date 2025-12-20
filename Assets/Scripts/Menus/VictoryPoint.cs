using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CanvasManager canvasManager = FindFirstObjectByType<CanvasManager>();
            if (canvasManager != null)
            {

                SceneManager.LoadScene("Victory");
            }
        }
    }
}
