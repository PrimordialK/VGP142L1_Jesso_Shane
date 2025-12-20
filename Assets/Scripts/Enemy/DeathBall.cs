using UnityEngine;
using UnityEngine.AI;


public class DeathBall : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;



    private Vector3 target;
    private NavMeshObstacle navObstacle;

    void Start()
    {
        target = pointB.position;
        navObstacle = GetComponent<NavMeshObstacle>();
        if (navObstacle != null)
        {
            navObstacle.carving = true; // Enable carving for dynamic obstacles
        }
    }

    void Update()
    {
        // Move between points
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            target = (target == pointA.position) ? pointB.position : pointA.position;
        }



    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                
            }
        }
    }


}

