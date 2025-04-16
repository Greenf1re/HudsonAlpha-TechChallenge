using UnityEngine;
using System.Collections;
using EasyButtons;
using UnityEngine.UI;
using UnityEngine.AI;

public class SoccerManager : MonoBehaviour
{
    public int score = 0;
    public int saves = 0; // Number of saves made by the goalkeeper (optional)
    public SoccerBall soccerBall;
    public NPCAnimator npcAnimator;
    public Transform ballStartPoint; // Starting position for the ball (optional)
    public Transform npcStartPoint; // Starting position for the NPC (optional)
    public Text scoreText; // UI Text to display the score (optional)
    public Text savesText; // UI Text to display the saves (optional)
    // public CollisionDetector collisionDetector; // Assign the CollisionDetector from goalpost/gloves
    public bool startOnSceneLoad = true; // Optionally start the game when the scene loads
    public float startDelay = 2f; // Delay before starting the game (optional)
    public bool loop = true; // Optionally loop the game after scoring

    private void Start()
    {
        // Subscribe to collision events to update the score
        // if (collisionDetector != null)
        // {
        //     collisionDetector.OnCollisionEvent.AddListener(OnScore);
        // }
        // Initialize the score text
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        if (savesText != null)
        {
            savesText.text = saves.ToString();
        }
        // Optionally start the game after a delay
        if (startOnSceneLoad)
        {
            StartCoroutine(StartGameAfterDelay(startDelay));
        }
    }
    // Coroutine to start the game after a delay
    private IEnumerator StartGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Optionally reset the game state
        ResetGame();
        // Launch the ball and start NPC action
        LaunchBall();
    }
    // Call this to reset the game state (e.g. when the player scores)


    // Call this to launch the ball and start NPC action
    [Button("Launch Ball")]
    public void LaunchBall()
    {
        if (soccerBall != null)
        {
            // soccerBall.Launch();
            // Start the NPC chasing and kicking the ball
            StartCoroutine(npcAnimator.ChaseAndKickBall(soccerBall));
        }
    }

    // Called when a collision is detected (i.e. a goal is scored)
    public void OnScore()
    {
        score++;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
        // Debug.Log("Score! Total: " + score);
        // Save the score (example using PlayerPrefs)
        // PlayerPrefs.SetInt("Score", score);
        // PlayerPrefs.Save();

        // Reset the game state (e.g. ball position)
        ResetGame();
    }
    public void OnSave(){
        saves++;
        if (savesText != null)
        {
            savesText.text = saves.ToString();
        }
        ResetGame();
    }
    // Reset game state after scoring
    public void ResetGame()
    {
        if (soccerBall != null)
        {
            soccerBall.ResetBall();
            npcAnimator.ResetNPC();
        }
        if (loop){
            StopAllCoroutines(); // Stop any running coroutines to avoid multiple launches
            StartCoroutine(StartGameAfterDelay(startDelay));
        }
        // Optionally, add further reset logic (e.g. resetting NPC position, timers, etc.)
    }
}
