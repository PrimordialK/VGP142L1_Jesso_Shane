using System.Collections;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [SerializeField] Vector3 localPositionValue = new Vector3(0, 0, 0);
    [SerializeField] Vector3 localRotationValue = new Vector3(-180, 0, 0);

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

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(1.0f);
        Physics.IgnoreCollision(bc, playerCollider, false);
    }
}
