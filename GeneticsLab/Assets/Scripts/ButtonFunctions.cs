using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour
{
	public Relay relay;

	public string joinCode;
	public InputField inputTextMeshPro;

	//private TouchScreenKeyboard keyboard;

	//public void ShowKeyboard()
	//{
	//	keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
	//}

	public void ClientInput()
	{
		joinCode = inputTextMeshPro.text;
	}

	public void Host()
	{
		print("Hosted");
		Debug.Log("Hosted");
		relay.AllocateRelay();
	}
	public void Client()
	{
		print("Joined as Client");
		Debug.Log("Joined as Client");
		ClientInput();
		relay.JoinRelay(joinCode);
	}
}
