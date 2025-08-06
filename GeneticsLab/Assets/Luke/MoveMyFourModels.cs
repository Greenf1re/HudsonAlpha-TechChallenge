using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Needed if you're using TextMeshPro

public class MoveMyFourModels : MonoBehaviour
{
    [Header("The models to move")]
    public List<GameObject> models;

    [Header("Available positions")]
    public List<Transform> positions;

    [Header("UI Text to display selected object name")]
    public TextMeshProUGUI targetText; // Drag your TMP Text here

    private GameObject chosenModel;

    // Static event that notifies listeners that a randomization is about to happen
    public static Action OnRandomizeAll;

    public void OnCorrectObjectGrabbed()
    {
        Debug.Log("Correct object was grabbed!");
        // Add custom behavior here if you want (score, VFX, etc.)
    }

    void Start()
    {
        MoveModelsToRandomPositions();
        PickRandomTarget();
    }

    void MoveModelsToRandomPositions()
    {
        if (models.Count == 0 || positions.Count == 0)
        {
            Debug.LogWarning("Please assign models and positions.");
            return;
        }

        if (positions.Count < models.Count)
        {
            Debug.LogWarning("Not enough positions for all models!");
            return;
        }

        List<Transform> availablePositions = new List<Transform>(positions);

        foreach (GameObject model in models)
        {
            int index = UnityEngine.Random.Range(0, availablePositions.Count);
            Transform chosenPos = availablePositions[index];
            model.transform.position = chosenPos.position;
            availablePositions.RemoveAt(index);
        }
    }

    void PickRandomTarget()
    {
        if (models.Count == 0)
        {
            Debug.LogWarning("No models assigned to pick from.");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, models.Count);
        chosenModel = models[randomIndex];

        Debug.Log("Target Object: " + chosenModel.name);

        if (targetText != null)
        {
            targetText.text = "Target Object: " + chosenModel.name;
        }
    }

    // Public method for other scripts to call to randomize and pick a new target
    public void RandomizeAll()
    {
        // Notify listeners first so they can drop held objects before we move them
        OnRandomizeAll?.Invoke();

        MoveModelsToRandomPositions();
        PickRandomTarget();
    }

    // Optional accessor
    public GameObject GetChosenModel()
    {
        return chosenModel;
    }
}
