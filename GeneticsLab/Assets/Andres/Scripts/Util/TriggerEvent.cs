using UnityEngine;
using UnityEngine.Events; // Necessary for UnityEvent

public class TriggerEvent : MonoBehaviour
{
    // Define the UnityEvent for entering the trigger
    [Tooltip("Event triggered when the player enters the trigger area.")]
    public UnityEvent onEnterTrigger;
    
    // Define the UnityEvent for exiting the trigger
    [Tooltip("Event triggered when the player exits the trigger area.")]
    public UnityEvent onExitTrigger;
    // public LayerMask tiggerMask;
    public bool excludePlayer = false; 

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

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the entry UnityEvent
            onEnterTrigger.Invoke();
        }
        else if (excludePlayer && !other.CompareTag("Player"))
        {
            // Invoke the entry UnityEvent for non-player objects
            onEnterTrigger.Invoke();
        }
        
        // if (other.gameObject.layer == tiggerMask)
        // {
        //     // Invoke the exit UnityEvent
        //     onExitTrigger.Invoke();
        // }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the exit UnityEvent
            onExitTrigger.Invoke();
        }
        else if (excludePlayer && !other.CompareTag("Player"))
        {
            // Invoke the exit UnityEvent for non-player objects
            onExitTrigger.Invoke();
        }
        // if (other.gameObject.layer == tiggerMask)
        // {
        //     // Invoke the exit UnityEvent
        //     onExitTrigger.Invoke();
        // }
    }
}