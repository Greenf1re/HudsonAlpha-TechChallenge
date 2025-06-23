using UnityEngine;
using System.Collections;

public class NPCAnimator : MonoBehaviour
{
    public Animator animator;
    public float moveSpeed = 3f;
    public Transform startingPoint;
    private SoccerBall currentBall;

    private void Start()
    {
        if (startingPoint != null)
            transform.position = startingPoint.position;
    }

    public IEnumerator ChaseAndKickBall(SoccerBall soccerBall)
    {
        currentBall = soccerBall; // store reference for later

        if (animator != null)
            animator.SetBool("isRunning", true);

        while (Vector3.Distance(transform.position, soccerBall.transform.position) > 1.0f)
        {
            transform.position = Vector3.MoveTowards(transform.position, soccerBall.transform.position, moveSpeed * Time.deltaTime);

            Vector3 direction = (soccerBall.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.SetTrigger("kick");
        }
    }

    // Method called by the Animation Event
    public void KickBall()
    {
        if (currentBall != null)
        {
            currentBall.Launch();
            currentBall = null; // Clear reference after use
        }
    }

    public void ResetNPC()
    {
        if (startingPoint != null)
            transform.position = startingPoint.position;

        if (animator != null)
            animator.SetBool("isRunning", false);

        StopAllCoroutines();
    }
}
