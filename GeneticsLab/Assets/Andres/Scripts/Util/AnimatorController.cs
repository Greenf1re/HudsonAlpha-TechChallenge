using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    // Reference to the Animator component
    private Animator animator;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
    }

    // This method toggles the state of a boolean parameter in the Animator
    public void ToggleBool(string parameterName)
    {
        // Check if the animator has the parameter, to avoid errors
        if (!animator || !animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
        {
            Debug.LogWarning("Animator does not have a boolean parameter named: " + parameterName);
            return;
        }

        // Get the current value of the parameter and toggle it
        bool currentValue = animator.GetBool(parameterName);
        animator.SetBool(parameterName, !currentValue);
    }
    public void SetBoolTrue(string parameterName)
    {
        // Check if the animator has the parameter, to avoid errors
        if (!animator || !animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
        {
            Debug.LogWarning("Animator does not have a boolean parameter named: " + parameterName);
            return;
        }

        // Get the current value of the parameter and toggle it
        bool currentValue = animator.GetBool(parameterName);
        animator.SetBool(parameterName, true);
    }
    public void SetBoolFalse(string parameterName)
    {
        // Check if the animator has the parameter, to avoid errors
        if (!animator || !animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
        {
            Debug.LogWarning("Animator does not have a boolean parameter named: " + parameterName);
            return;
        }

        // Get the current value of the parameter and toggle it
        bool currentValue = animator.GetBool(parameterName);
        animator.SetBool(parameterName, false);
    }
}

// Extension method to check if an Animator has a specific parameter of a certain type
public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator animator, string parameterName, AnimatorControllerParameterType type)
    {
        foreach (var param in animator.parameters)
        {
            if (param.type == type && param.name == parameterName)
                return true;
        }
        return false;
    }
}