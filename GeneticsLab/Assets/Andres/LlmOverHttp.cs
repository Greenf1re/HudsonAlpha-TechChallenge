using System;
using System.Collections;
using System.Collections.Generic;
using EasyButtons;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class LlmOverHttp : MonoBehaviour
{
    [Serializable]
    public class LlmRequest
    {
        public string model = "medllama2"; //medllama2 or meditron
        public string prompt = "A 35-year-old woman presents with a persistent dry cough, shortness of breath, and fatigue. She is initially suspected of having asthma, but her spirometry results do not improve with bronchodilators. What could be the diagnosis?";
        public bool stream = false;
    }
    [Serializable]
    public class Response
    {
        public string model;
        public string created_at;
        public string response;
        public bool done;
        public string context;
        public string total_duration;
        public string load_duration;
        public string prompt_eval_duration;
        public string eval_count;
        public string eval_duration;

    }
    public string ip = "127.0.0.1";
    public int port = 11434;
    public string postUrl = "/api/generate";
    public Response response = new Response();
    public LlmRequest llmRequest = new LlmRequest();
    public TextMeshPro text;
    public UnityWebRequest CreateApiGetRequest(string actionUrl, object body = null)
    {
        return CreateApiRequest(actionUrl, UnityWebRequest.kHttpVerbGET, body);
    }
    
    public UnityWebRequest CreateApiPostRequest(string actionUrl, object body = null)
    {
        return CreateApiRequest(actionUrl, UnityWebRequest.kHttpVerbPOST, body);
    }
    
    UnityWebRequest CreateApiRequest(string url, string method, object body)
    {
        string bodyString = null;
        if (body is string)
        {
            bodyString = (string)body;
        }
        else if (body != null)
        {
            bodyString = JsonUtility.ToJson(body);
        }
    
        var request = new UnityWebRequest();
        request.url = url;
        request.method = method;
        request.downloadHandler = new DownloadHandlerBuffer();
        request.uploadHandler = new UploadHandlerRaw(string.IsNullOrEmpty(bodyString) ? null : System.Text.Encoding.UTF8.GetBytes(bodyString));
        request.SetRequestHeader("Accept", "application/json");
        request.SetRequestHeader("Content-Type", "application/json");
        request.timeout = 60;
        return request;
    }
    void Reset()
    {
        StartCoroutine(SendPost2());
    }
    [Button]
    public void SendRequest(){
        StartCoroutine(SendPost2());
    }
    IEnumerator SendPost2(){
        var postData = llmRequest;
        Debug.Log("DATA " + postData.model + " " + postData.prompt + " IP " + $"http://{ip}:{port}{postUrl}");
        var request = CreateApiPostRequest($"http://{ip}:{port}{postUrl}", postData);
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST request sent successfully");
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Error sending POST request: {request.error}");
        }
        // Print the response
        // Debug.Log("Response: " + request.downloadHandler.text);
        // Get just the response field from the JSON
        response = JsonUtility.FromJson<Response>(request.downloadHandler.text);
        text.text = response.response;
        // Debug.Log("Response: " + response.response);
    }
    // IEnumerator SendPostRequest()
    // {
    //     string url = $"http://{ip}:{port}/api/generate";
    //     string json = $"{{\"model\": \"{model}\", \"prompt\": \"{prompt}\"}}";
    //     var postData = System.Text.Encoding.UTF8.GetBytes(json);
    //     // string json = @"{
    //     //     ""model"": ""medllama2"",
    //     //     ""prompt"": ""A 35-year-old woman presents with a persistent dry cough, shortness of breath, and fatigue. She is initially suspected of having asthma, but her spirometry results do not improve with bronchodilators. What could be the diagnosis?""
    //     // }";
    //     Debug.Log(postData);
    //     using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, json))
    //     {
    //         request.SetRequestHeader("Content-Type", "application/json");

    //         yield return request.SendWebRequest();

    //         if (request.result == UnityWebRequest.Result.Success)
    //         {
    //             Debug.Log("POST request sent successfully");
    //         }
    //         else
    //         {
    //             Debug.LogError($"Error sending POST request: {request.error}");
    //         }
    //         // Print the response
    //         Debug.Log("Response: " + request.downloadHandler.text);
    //     }
    // }

    void Update()
    {
        
    }
}
