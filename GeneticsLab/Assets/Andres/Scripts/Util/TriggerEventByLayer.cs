using UnityEngine;
using UnityEngine.Events; // Necessary for UnityEvent

public class TriggerEventByLayer : MonoBehaviour
{
    // Define the UnityEvent for entering the trigger
    [Tooltip("Event triggered when the player enters the trigger area.")]
    public UnityEvent onEnterTrigger;
    
    // Define the UnityEvent for exiting the trigger
    [Tooltip("Event triggered when the player exits the trigger area.")]
    public UnityEvent onExitTrigger;
    // public LayerMask tiggerMask;
    public LayerMask triggerMask; // LayerMask to specify which layers can trigger the event

    private void Reset()
    {
        // Initialize the UnityEvents if they haven't been assigned in the editor
        if (onEnterTrigger == null)
        {
            onEnterTrigger = new UnityEvent();
        }
        if (onExitTrigger == null)
        {
            onExitTrigger = new UnityEvent();
        }
    }
    // OnTriggerEnter is called when another collider enters the trigger collider attached to this object
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is on the specified layer
        if ((triggerMask & (1 << other.gameObject.layer)) != 0)
        {
            // Invoke the entry UnityEvent
            onEnterTrigger.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is on the specified layer
        if ((triggerMask & (1 << other.gameObject.layer)) != 0)
        {
            // Invoke the exit UnityEvent
            onExitTrigger.Invoke();
        }
    }
}