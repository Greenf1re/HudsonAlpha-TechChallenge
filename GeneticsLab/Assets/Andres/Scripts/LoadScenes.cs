using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadScenes : MonoBehaviour
{
    public GameObject loadingScreen;

    public GetSlider getSlider;
    public Slider progressBar;
    public Text progressText;
    public bool noUI = false;
    // Function to be called when loading a scene by name
    public void LoadSceneByName(string sceneName)
    {
        if (noUI) StartCoroutine(LoadSceneAsync(sceneName));
        else StartCoroutine(LoadSceneAsyncImmediate(sceneName));
    }

    // Function to be called when loading a scene by build index
    public void LoadSceneByIndex(int sceneBuildIndex)
    {
        if (noUI) StartCoroutine(LoadSceneAsync(sceneBuildIndex));
        else StartCoroutine(LoadSceneAsyncImmediate(sceneBuildIndex));
    }
    IEnumerator LoadSceneAsyncImmediate(object sceneIdentifier)
    {
        Debug.Log("Loading scene...");
        AsyncOperation operation;
        if (sceneIdentifier is string sceneName)
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
        }
        else if (sceneIdentifier is int sceneBuildIndex)
        {
            operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        }
        else
        {
            yield break; // Exit if the identifier is neither a string nor an integer
        }

        loadingScreen.SetActive(true); // Show loading screen
        Debug.Log("Loading screen active");
        while (!operation.isDone)
        {
            yield return null; // Wait a frame before continuing the loop
        }
        Debug.Log("Loading complete");
        loadingScreen.SetActive(false); // Hide loading screen after loading is complete
    }
    IEnumerator LoadSceneAsync(object sceneIdentifier)
    {
        Debug.Log("Loading scene...");
        if (progressBar == null)
        {
            progressBar = getSlider.slider;
        }
        if (progressText == null)
        {
            progressText = getSlider.text;
        }
        AsyncOperation operation;
        if (sceneIdentifier is string sceneName)
        {
            operation = SceneManager.LoadSceneAsync(sceneName);
        }
        else if (sceneIdentifier is int sceneBuildIndex)
        {
            operation = SceneManager.LoadSceneAsync(sceneBuildIndex);
        }
        else
        {
            yield break; // Exit if the identifier is neither a string nor an integer
        }

        loadingScreen.SetActive(true); // Show loading screen
        Debug.Log("Loading screen active");
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // operation.progress goes from 0 to 0.9
            progressBar.value = progress;
            // progressText.text = $"{progress * 100}%";
            
            yield return null; // Wait a frame before continuing the loop
        }
        Debug.Log("Loading complete");
        loadingScreen.SetActive(false); // Hide loading screen after loading is complete
    }
}