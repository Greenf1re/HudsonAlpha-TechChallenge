using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Object.DontDestroyOnLoad example.
//
// This script example manages the playing audio. The GameObject with the
// "music" tag is the BackgroundMusic GameObject. The AudioSource has the
// audio attached to the AudioClip.

public class DontDestroyOnLoad : MonoBehaviour
{
    void Awake()
    {

        DontDestroyOnLoad(NetworkManager.Singleton.gameObject);
    }
}