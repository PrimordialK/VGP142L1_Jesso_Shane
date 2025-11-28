using UnityEngine;

public class Sword : WeaponBase
{
    [SerializeField] private float projectileSpeed = 15f;





    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("Sword hit an enemy (trigger)!");
                enemy.Die();
            }
        }
    }

}
