using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class GrabbableChecker : MonoBehaviour
{
    private MoveMyFourModels manager;
    private HandGrabInteractable handGrab;
    private LayerSwitchOnGrab layerSwitcher;

    [Header("Controller (OVR) Settings")]
    public Transform leftController;
    public Transform rightController;

    [Tooltip("Distance for controller raycast.")]
    public float raycastDistance = 2f;

    [Tooltip("Which OVRInput.Button(s) count as a grab.")]
    public OVRInput.Button leftGrabButtons = OVRInput.Button.PrimaryHandTrigger | OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Button rightGrabButtons = OVRInput.Button.SecondaryHandTrigger | OVRInput.Button.SecondaryIndexTrigger;

    [Tooltip("Use GetDown (press) instead of Get (hold) for grab detection.")]
    public bool useGetDown = true;

    [Header("No-push options")]
    [Tooltip("Make the rigidbody kinematic while held.")]
    public bool makeKinematicWhileHeld = true;

    [Header("Feedback")]
    [Tooltip("Play this AudioClip via the AudioSource when the correct object is grabbed.")]
    public AudioSource audioSource;
    [Tooltip("Optional: override the AudioSource.clip by assigning a clip here.")]
    public AudioClip correctClip;

    // internals
    private Collider myCollider;
    private Rigidbody rb;
    private bool isGrabbed = false;
    private Transform heldBy = null; // controller transform if held by controller, null if held by hand
    private OVRInput.Button heldButtons = OVRInput.Button.None;
    private OVRInput.Controller heldControllerEnum = OVRInput.Controller.None;
    private bool successHandledForThisGrab = false;

    private void Awake()
    {
        manager = FindObjectOfType<MoveMyFourModels>();
        handGrab = GetComponent<HandGrabInteractable>();
        layerSwitcher = GetComponent<LayerSwitchOnGrab>();
        myCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

        if (myCollider == null) Debug.LogWarning("GrabbableChecker: no Collider found on object.");
        if (rb == null) Debug.LogWarning("GrabbableChecker: no Rigidbody found on object.");
    }

    private void OnEnable()
    {
        if (handGrab != null)
        {
            handGrab.WhenPointerEventRaised += OnPointerEvent;
        }

        // Subscribe to the randomizer event so we can force a drop
        MoveMyFourModels.OnRandomizeAll += HandleRandomizeAll;
    }

    private void OnDisable()
    {
        if (handGrab != null)
        {
            handGrab.WhenPointerEventRaised -= OnPointerEvent;
        }

        MoveMyFourModels.OnRandomizeAll -= HandleRandomizeAll;
    }

    private void Start()
    {
        // Try to auto-find controller transforms if they are not assigned.
        if (leftController == null)
        {
            var leftGO = GameObject.Find("LeftHandAnchor") ?? GameObject.Find("LeftHandController") ?? GameObject.Find("LeftController");
            if (leftGO != null) leftController = leftGO.transform;
        }
        if (rightController == null)
        {
            var rightGO = GameObject.Find("RightHandAnchor") ?? GameObject.Find("RightHandController") ?? GameObject.Find("RightController");
            if (rightGO != null) rightController = rightGO.transform;
        }
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        // HandGrab selects/unselects via pointer events
        if (evt.Type == PointerEventType.Select)
        {
            PickupByHand();
            HandleSuccessIfTarget();
        }
        else if (evt.Type == PointerEventType.Unselect)
        {
            ReleaseFromHand();
        }
    }

    private void Update()
    {
        // Controller-based checks (press to pick up)
        CheckController(leftController, leftGrabButtons, OVRInput.Controller.LTouch);
        CheckController(rightController, rightGrabButtons, OVRInput.Controller.RTouch);

        // If currently held by a controller, check release via GetUp
        if (isGrabbed && heldBy != null)
        {
            if (heldControllerEnum != OVRInput.Controller.None && OVRInput.GetUp(heldButtons, heldControllerEnum))
            {
                Release();
            }
        }
    }

    private void CheckController(Transform controllerTransform, OVRInput.Button buttons, OVRInput.Controller controller)
    {
        if (controllerTransform == null) return;

        bool pressed = useGetDown ? OVRInput.GetDown(buttons, controller) : OVRInput.Get(buttons, controller);
        if (!pressed) return;

        // Raycast forward from controller
        Ray ray = new Ray(controllerTransform.position, controllerTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            if (hit.collider == myCollider)
            {
                PickupByController(controllerTransform, buttons, controller);
                HandleSuccessIfTarget();
            }
        }
    }

    private void PickupByHand()
    {
        if (isGrabbed) return;
        isGrabbed = true;
        heldBy = null;
        heldControllerEnum = OVRInput.Controller.None;
        heldButtons = OVRInput.Button.None;
        successHandledForThisGrab = false;

        // Make kinematic to avoid pushing
        if (rb != null && makeKinematicWhileHeld)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void ReleaseFromHand()
    {
        if (!isGrabbed || heldBy != null) return;
        Release();
    }

    private void PickupByController(Transform controller, OVRInput.Button buttons, OVRInput.Controller controllerEnum)
    {
        if (isGrabbed) return;
        isGrabbed = true;
        heldBy = controller;
        heldButtons = buttons;
        heldControllerEnum = controllerEnum;
        successHandledForThisGrab = false;

        // Parent to controller so it moves with it
        transform.SetParent(controller, true);

        // Ask LayerSwitchOnGrab to set grabbed layer (if present)
        if (layerSwitcher != null)
            layerSwitcher.ApplyGrabbedLayer();

        // Make kinematic to avoid pushing the player
        if (rb != null && makeKinematicWhileHeld)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    // Public helper so the randomizer event (or anything else) can force a drop
    public void ForceRelease()
    {
        if (isGrabbed)
            Release();
    }

    private void Release()
    {
        // Unparent if parented to controller
        if (heldBy != null)
        {
            transform.SetParent(null, true);
        }

        // Ask LayerSwitchOnGrab to restore layer (if present)
        if (layerSwitcher != null)
            layerSwitcher.RestoreOriginalLayer();

        // restore physics
        if (rb != null && makeKinematicWhileHeld)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // reset state
        isGrabbed = false;
        heldBy = null;
        heldButtons = OVRInput.Button.None;
        heldControllerEnum = OVRInput.Controller.None;
        successHandledForThisGrab = false;
    }

    private void HandleSuccessIfTarget()
    {
        if (manager == null) manager = FindObjectOfType<MoveMyFourModels>();
        if (manager == null) return;

        if (gameObject == manager.GetChosenModel() && !successHandledForThisGrab)
        {
            // Notify manager
            manager.OnCorrectObjectGrabbed();

            // Play audio
            if (audioSource != null)
            {
                if (correctClip != null)
                    audioSource.PlayOneShot(correctClip);
                else if (audioSource.clip != null)
                    audioSource.Play();
            }

            // randomize via manager (this will also trigger the OnRandomizeAll event,
            // which notifies all GrabbableChecker instances to ForceRelease)
            manager.RandomizeAll();

            successHandledForThisGrab = true;
        }
    }

    // Called when the randomizer starts so we drop the object immediately
    private void HandleRandomizeAll()
    {
        ForceRelease();
    }
}
