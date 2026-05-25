using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class BallManager : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float minSpeed = 8f;
    [SerializeField] private float maxSpeed = 18f;

    [Header("Collision")]
    [SerializeField] private float bounceEnergy = 1.0f;
    [SerializeField] private LayerMask bounceLayer;

    private Rigidbody rb;
    private Vector3 currentDirection;
    private bool isLaunched;

    private void Awake()
    {

        rb = GetComponent<Rigidbody>();

    }

    private void FixedUpdate()
    {
        if (!isLaunched) return;

        float currentSpeed = rb.velocity.magnitude;

        if (currentSpeed <= 0.01f) return;

        currentDirection = rb.velocity.normalized;

        if (currentSpeed < minSpeed)
        {
            rb.velocity = currentDirection * minSpeed;
        }
        else if (currentSpeed > maxSpeed)
        {
            rb.velocity = currentDirection * maxSpeed;
        }
    }

    public void Launch(Vector3 direction, float speed)
    {
        isLaunched = true;

        currentDirection = direction.normalized;
        float finalSpeed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        rb.velocity = currentDirection * finalSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isLaunched) return;

        if (((1 << collision.gameObject.layer) & bounceLayer) == 0)
            return;

        ContactPoint contact = collision.contacts[0];

        EnvironmentObject hitObject = collision.gameObject.GetComponent<EnvironmentObject>();

        if (hitObject != null)
        {
            hitObject.TakeHit();
        }

    }

    public void StopBall()
    {
        isLaunched = false;
        currentDirection = Vector3.zero;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}