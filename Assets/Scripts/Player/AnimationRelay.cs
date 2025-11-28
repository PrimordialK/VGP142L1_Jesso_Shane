using UnityEngine;

public class AnimationRelay : MonoBehaviour
{
    public void ShootEquippedWeapon()
    {
        var parentController = GetComponentInParent<PlayerController>();
        if (parentController != null)
            parentController.ShootEquippedWeapon();
    }
}