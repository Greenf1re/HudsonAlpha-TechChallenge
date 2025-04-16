using UnityEngine;
using System.Collections;

public class NPCAnimator : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 3f;
    public Transform startingPoint; // Starting position for the NPC

    private void Start()
    {
        // Optionally place the NPC at its starting point
        if (startingPoint != null)
        {
            transform.position = startingPoint.position;
        }
    }

    // Coroutine that moves the NPC to the ball, plays run and kick animations
    public IEnumerator ChaseAndKickBall(SoccerBall soccerBall)
    {
        Transform ball = soccerBall.transform;
        // Start running
        if (animator != null)
            animator.SetBool("isRunning", true);

        
        while (Vector3.Distance(transform.position, ball.position) > 1.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ball.position, moveSpeed * Time.deltaTime);
            // Optionally, rotate the NPC to face the ball
            Vector3 direction = (ball.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
            // Move toward the ball until close enough (adjust threshold as needed)
            if (Vector3.Distance(transform.position, ball.position) <= 1.5f){
                // start kick animation earlier
                if (animator != null){
                    animator.SetTrigger("kick");
                    animator.SetBool("isRunning", false);
                }
            }
            yield return null;
        }

        // Stop running and trigger kick animation
        if (animator != null)
        {
            // animator.SetBool("isRunning", false);
            // animator.SetTrigger("kick");
            soccerBall.Launch(); // Launch the ball (assuming this method exists in SoccerBall)
        }

        // Wait for the kick animation to finish (adjust duration as needed)
        yield return new WaitForSeconds(1f);

        // Optionally, reset the NPC to its starting position
        // if (startingPoint != null)
        // {
        //     transform.position = startingPoint.position;
        // }
    }
    public void ResetNPC()
    {
        // Reset the NPC to its starting position and stop any animations
        if (startingPoint != null)
        {
            transform.position = startingPoint.position;
        }
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            // animator.SetTrigger("reset"); // Optional: trigger a reset animation
        }
        // stop coroutines
        StopAllCoroutines();
    }
}
