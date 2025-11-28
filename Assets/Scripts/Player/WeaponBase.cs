
using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] public Vector3 localPositionValue = new Vector3(0, 0, 0);
    [SerializeField] public Vector3 localRotationValue = new Vector3(-180, 0, 0);

    [Header("Shooting")]
    [SerializeField] public GameObject projectilePrefab; // Assign unique prefab per weapon
    [SerializeField] public Transform shootOrigin;       // Assign in Inspector (e.g., tip of weapon)

    public bool equipped = false;
    public bool holstered = false;

    Rigidbody rb;
    BoxCollider bc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public void Equip(Collider playerCollider, Transform weaponAttachPoint)
    {
        rb.isKinematic = true;
        bc.isTrigger = true;
        transform.SetParent(weaponAttachPoint);
        transform.localPosition = localPositionValue;
        transform.localRotation = Quaternion.Euler(localRotationValue);
        Physics.IgnoreCollision(bc, playerCollider, true);
    }

    public void Drop(Collider playerCollider)
    {
        rb.isKinematic = false;
        bc.isTrigger = false;
        transform.SetParent(null);
        rb.AddForce(playerCollider.transform.forward * 2f, ForceMode.Impulse);
        StartCoroutine(DropCooldown(playerCollider));
    }

    public void Equipped(Collider playerCollider, Transform weaponAttachPoint)
    {
        Equip(playerCollider, weaponAttachPoint);
        equipped = true;
        holstered = false;
        Debug.Log("Equipped weapon: " + name);
    }

    public void Holstered(Collider playerCollider, Transform weaponAttachPoint)
    {
        Equip(playerCollider, weaponAttachPoint);
        equipped = false;
        holstered = true;
        Debug.Log("Holstered weapon: " + name);
    }

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(1.0f);
        Physics.IgnoreCollision(bc, playerCollider, false);
    }
}
