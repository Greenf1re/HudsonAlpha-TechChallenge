using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

[RequireComponent(typeof(HandGrabInteractable))]
public class LayerSwitchOnGrab : MonoBehaviour
{
    [Tooltip("The layer name to switch to while grabbed.")]
    public string grabbedLayerName = "GrabbedObject";

    private int grabbedLayer;
    private int originalLayer;
    private HandGrabInteractable handGrab;

    private void Awake()
    {
        handGrab = GetComponent<HandGrabInteractable>();
        originalLayer = gameObject.layer;

        grabbedLayer = LayerMask.NameToLayer(grabbedLayerName);
        if (grabbedLayer == -1)
        {
            Debug.LogError($"Layer '{grabbedLayerName}' not found. Please create it in Tags & Layers.");
        }
    }

    private void OnEnable()
    {
        if (handGrab != null)
        {
            handGrab.WhenPointerEventRaised += OnPointerEvent;
        }
    }

    private void OnDisable()
    {
        if (handGrab != null)
        {
            handGrab.WhenPointerEventRaised -= OnPointerEvent;
        }
    }

    private void OnPointerEvent(PointerEvent evt)
    {
        if (grabbedLayer == -1) return;

        if (evt.Type == PointerEventType.Select) // Grab start
        {
            SetLayerRecursively(gameObject, grabbedLayer);
        }
        else if (evt.Type == PointerEventType.Unselect) // Grab end
        {
            SetLayerRecursively(gameObject, originalLayer);
        }
    }

    // Public helper so other scripts (controller grab path) can request layer switching
    public void ApplyGrabbedLayer()
    {
        if (grabbedLayer == -1) return;
        SetLayerRecursively(gameObject, grabbedLayer);
    }

    public void RestoreOriginalLayer()
    {
        SetLayerRecursively(gameObject, originalLayer);
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
