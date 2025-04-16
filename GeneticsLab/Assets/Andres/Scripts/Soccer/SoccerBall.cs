using UnityEngine;
using System.Collections;
public class SoccerBall : MonoBehaviour
{
    public Rigidbody rb;
    public Transform goalPlane;  // Reference to the goalpost plane (should have a Collider)
    public float launchForce = 10f;
    public float upwardForce = 5f;
    public float delayBeforeReset = 2f; // Delay before resetting the ball (optional)

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        // Store the initial state for resetting the ball
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    public void Launch()
    {
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

        // Pick a random point on the goal plane's collider bounds
        Vector3 min = goalCollider.bounds.min;
        Vector3 max = goalCollider.bounds.max;
        Vector3 randomTarget = new Vector3(
            Random.Range(min.x, max.x),
            Random.Range(min.y, max.y),
            Random.Range(min.z, max.z)
        );

        // Compute the direction to the target
        Vector3 direction = (randomTarget - transform.position).normalized;

        // Create a force vector with an upward component for a parabolic trajectory
        Vector3 force = direction * launchForce + Vector3.up * upwardForce;

        rb.velocity = Vector3.zero; // Reset any previous velocity
        rb.AddForce(force, ForceMode.Impulse);
        rb.AddTorque(Vector3.up * 10f, ForceMode.Impulse); // Optional: add some spin
        rb.AddTorque(Vector3.right * 10f, ForceMode.Impulse); // Optional: add some spin    

        // Debug.Log("Ball launched towards " + randomTarget);
        // StartResetTimer(); // Start the reset timer
    }

    // Reset ball to its initial position and rotation
    public void ResetBall(Transform position = null)
    {
        ResetRigidbody();

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

        // Reset the Rigidbody's velocity and angular velocity
    }
    private void ResetRigidbody()
    {
        rb.isKinematic = true; // Disable physics while resetting
        // rb.velocity = Vector3.zero;
        // rb.angularVelocity = Vector3.zero;
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        rb.isKinematic = false; // Re-enable physics after resetting
    }
    // Timer to reset the ball after a certain time (optional)
    // private IEnumerator ResetBallAfterDelay(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     ResetBall();
    // }
    // // Optionally, you can call this method to start the timer
    // public void StartResetTimer(float delay)
    // {
    //     StartCoroutine(ResetBallAfterDelay(delay));
    // }
    // public void StartResetTimer()
    // {
    //     StartCoroutine(ResetBallAfterDelay(delayBeforeReset));
    // }
}
