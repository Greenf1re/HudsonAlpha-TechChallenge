using UnityEngine;

public class SoccerBall : MonoBehaviour
{
    public Rigidbody rb;
    public Transform goalPlane;  // The goal area to aim for
    public float launchForce = 10f;
    public float upwardForce = 5f;
    public float delayBeforeReset = 2f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Launch()
    {
        Debug.Log("KickBall event triggered.");

        if (goalPlane == null)
        {
            Debug.LogError("Goal plane not assigned!");
            return;
        }

        Collider goalCollider = goalPlane.GetComponent<Collider>();
        if (goalCollider == null)
        {
            Debug.LogError("Goal plane does not have a Collider!");
            return;
        }

        // Pick a random point on the goal plane
        Bounds bounds = goalCollider.bounds;
        Vector3 randomTarget = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );

        // Calculate force
        Vector3 direction = (randomTarget - transform.position).normalized;
        Vector3 force = direction * launchForce + Vector3.up * upwardForce;

        // Reset and apply force
        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * 10f + Vector3.right * 10f, ForceMode.Impulse);

        Debug.Log($"Ball launched towards {randomTarget}");
    }

    public void ResetBall(Transform position = null)
    {
        rb.isKinematic = true; // Disable physics temporarily

        if (position != null)
        {
            transform.position = position.position;
            transform.rotation = position.rotation;
        }
        else
        {
            transform.position = initialPosition;
            transform.rotation = initialRotation;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = false; // Reactivate physics
    }
}
