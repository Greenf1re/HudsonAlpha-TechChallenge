using UnityEngine;
using UnityEngine.Events; // Necessary for UnityEvent

public class TriggerEvent : MonoBehaviour
{
    // Define the UnityEvent for entering the trigger
    public UnityEvent onEnterTrigger;
    
    // Define the UnityEvent for exiting the trigger
    public UnityEvent onExitTrigger;

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
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the object exiting the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Invoke the exit UnityEvent
            onExitTrigger.Invoke();
        }
    }
}