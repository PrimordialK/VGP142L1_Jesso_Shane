using UnityEngine;

public class ChildofPlayer : MonoBehaviour
{
    [Tooltip("If true, this object will be parented to the player.")]
    public bool setAsChild = true;

    private Transform playerTransform;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            if (setAsChild)
            {
                transform.SetParent(playerTransform);
            }
        }
        else
        {
            Debug.LogWarning("Player not found in scene. Make sure the player GameObject is tagged 'Player'.");
        }
    }

    void Update()
    {
        // If not parented, you can make this object follow the player manually
        if (!setAsChild && playerTransform != null)
        {
            transform.position = playerTransform.position;
        }
    }
}
