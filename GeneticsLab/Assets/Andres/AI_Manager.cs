using EasyButtons;
using HuggingFace.API.Examples;
using UnityEngine;
using EasyButtons;
// using UnityEngine.InputSystem;

public class AI_Manager : MonoBehaviour
{
    public LlmOverHttp llm;
    public SpeechRecognition sr;
    public bool isListening = false;
    public GameObject ListeningText;
    public GameObject ThinkingText;
    private float previousAxisValue = 0.0f;
    private void Start()
    {
        if (!llm)
        {
            llm = GetComponent<LlmOverHttp>();
        }
        if (!sr)
        {
            sr = GetComponent<SpeechRecognition>();
        }
    }
    [Button]
    public void StartListening()
    {
        isListening = true;
        ListeningText.SetActive(true);
        ThinkingText.SetActive(false);
        sr.StartRecording();
    }
    [Button]
    public void StopListening()
    {
        isListening = false;
        ListeningText.SetActive(false);
        ThinkingText.SetActive(true);
        sr.StopRecording();
    }


}