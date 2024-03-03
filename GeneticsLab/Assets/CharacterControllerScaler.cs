using UnityEngine;

public class CharacterControllerScaler : MonoBehaviour
{
    public CharacterController characterController;
    public Transform playerCamera; // Assign the VR Camera representing the player's head

    // Adjustable parameters for minimum height and scale factor
    public float minHeight = 1.5f;
    public float maxHeight = 2.0f;
    public float scaleMultiplier = 1.0f;

    void Start()
    {
        if (!characterController)
        {
            characterController = GetComponent<CharacterController>();
        }

        ScaleCharacterController();
    }

    void ScaleCharacterController()
    {
        if (playerCamera == null)
        {
            Debug.LogError("Player camera not assigned.");
            return;
        }

        // Calculate the player's height based on the camera's position
        float playerHeight = playerCamera.localPosition.y;

        // Clamp player height within a reasonable range
        playerHeight = Mathf.Clamp(playerHeight, minHeight, maxHeight);

        // Optionally, adjust scale based on a multiplier for game-specific scaling
        playerHeight *= scaleMultiplier;

        // Set the Character Controller height
        characterController.height = playerHeight;

        // Adjust the center of the Character Controller as the height changes
        characterController.center = new Vector3(characterController.center.x, playerHeight / 2, characterController.center.z);
    }

    // Optionally, if you need to update the scale dynamically (e.g., if the player can crouch), you can call ScaleCharacterController() from Update() or via a specific event.
}