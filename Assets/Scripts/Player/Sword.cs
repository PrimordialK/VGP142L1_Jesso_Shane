using UnityEngine;

public class Sword : WeaponBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start() => base.Start();

    // Update is called once per frame
    void Update()
    {
        
    }

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
