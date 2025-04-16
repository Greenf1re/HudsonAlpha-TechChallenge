using UnityEngine;
// using UnityEngine.InputSystem;

public class VRController : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;
    public GameObject menu;
    public GameObject levelsMenu;
    public OVRVirtualKeyboardSampleControls keyboardController;
    public OVRVirtualKeyboard OVRkeyboard;
    public float speed = 5.0f;
    public float rotationSpeed = 5.0f;
    public float jumpHeight = 2.0f;
    public float gravityValue = -9.81f;

    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float rotationY = 0;

    public float minHeight = 1.5f;
    public float maxHeight = 2.0f;
    public float scaleMultiplier = 1.0f;
    private void Start()
    {
        if (!controller)
        {
            controller = GetComponent<CharacterController>();
        }
        // ScaleCharacterController();
        // Make OVRKeyboard child of scene
        OVRkeyboard.transform.SetParent(null);
    }

    private void Update()
    {
        // Check if the player is grounded
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Get input from the left joystick for movement
        Vector2 movement = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        
        // Move the player
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = playerCamera.forward * move.z + playerCamera.right * move.x;
        move.y = 0; // Ensure we don't accidentally apply vertical movement here
        controller.Move(move * Time.deltaTime * speed);

        // Get input from the right joystick for rotation
        Vector2 rotation = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        rotationY += rotation.x * rotationSpeed;
        transform.eulerAngles = new Vector3(0, rotationY, 0);

        // Jumping with button A
        if (OVRInput.GetDown(OVRInput.Button.One) && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Open menu with the menu button
        if (OVRInput.GetDown(OVRInput.Button.Start))
        {
            // Implement your menu logic here
            Debug.Log("Menu button pressed.");
            if(levelsMenu.activeSelf)
            {
                levelsMenu.SetActive(false);
            }
            if(menu.activeSelf)
            {
                keyboardController.MoveKeyboardFar();
                keyboardController.MoveKeyboard();
                keyboardController.HideKeyboard();
                menu.SetActive(false);
            }
            else
            {
                keyboardController.ShowKeyboard();
                menu.SetActive(true);
            }
            // menu.SetActive(!menu.activeSelf);
        }
        //Open levels menu with the Y button
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            if(menu.activeSelf)
            {
                menu.SetActive(false);
                keyboardController.HideKeyboard();
            }
            levelsMenu.SetActive(!levelsMenu.activeSelf);
        }
        // Alter time wihtthe B button
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0.25f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
        // Speed up if trigger is held
        if (OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger) > 0.1f)
        {
            speed = 10.0f;
        }
        else
        {
            speed = 5.0f;
        }
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
        controller.height = playerHeight;

        // Adjust the center of the Character Controller as the height changes
        controller.center = new Vector3(controller.center.x, playerHeight / 2, controller.center.z);
    }
}