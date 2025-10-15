using UnityEngine;

public class PlayerController : MonoBehaviour
{
 public float speed = 5.0f;
    Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 moveVel = new Vector3(hInput * speed, rb.linearVelocity.y, vInput * speed);

        rb.linearVelocity = moveVel;
    }
}
