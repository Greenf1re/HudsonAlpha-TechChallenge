using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class Relay : MonoBehaviour
{
    [SerializeField]
    private short maxPlayers = 4;
    private string joinCode;
	private bool isHost = false;
    public TMPro.TextMeshProUGUI textMeshProUGUI;

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        Debug.Log("Unity Services Initialized.");

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed In: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Signed in anonymously. PlayerID: " + AuthenticationService.Instance.PlayerId);

        // Print the initial state of the NetworkManager
        PrintNetworkManagerState();
    }

    public async void AllocateRelay()
    {
        // Print the current state before attempting to start the host
		if(!isHost){
			isHost = true;
			PrintNetworkManagerState();

			if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
			{
				Debug.LogWarning("Cannot allocate relay. A network instance is already running.");
				return;
			}

			try
			{
				Debug.Log("Host - Creating an allocation.");

				Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
				joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
				textMeshProUGUI.text = joinCode;
				Debug.Log("Join Code: " + joinCode);

				RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
				NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

				NetworkManager.Singleton.StartHost();
				Debug.Log("Network host started.");
			}
			catch (RelayServiceException e)
			{
				Debug.LogError("Relay Allocation Failed: " + e.Message);
			}
		}
    }

    public async void JoinRelay(string joinCode)
    {
        // Print the current state before attempting to start the client
        PrintNetworkManagerState();

        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
        {
            Debug.LogWarning("Cannot join relay. A network instance is already running.");
            return;
        }

        try
        {
            Debug.Log("Client - Joining Relay with code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            Debug.Log("Network client started.");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Relay Join Failed: " + e.Message);
        }
    }

    // Helper method to print the current state of the NetworkManager
    private void PrintNetworkManagerState()
    {
        if(NetworkManager.Singleton == null)
        {
            Debug.LogWarning("NetworkManager is null.");
            return;
        }
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Current NetworkManager state: Host");
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Current NetworkManager state: Server");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Current NetworkManager state: Client");
        }
        else
        {
            Debug.Log("Current NetworkManager state: Inactive");
        }
    }
}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class Relay : MonoBehaviour
{
	[SerializeField]
	private short maxPlayers = 4;
	private string joinCode;
	public TMPro.TextMeshProUGUI textMeshProUGUI;

	private async void Start()
	{
		await UnityServices.InitializeAsync();

		AuthenticationService.Instance.SignedIn += () =>
		{
			Debug.Log("Signed In" + AuthenticationService.Instance.PlayerId);
		};
		await AuthenticationService.Instance.SignInAnonymouslyAsync();
		// Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");
		PrintNetworkManagerState();
	}

	public async void AllocateRelay()
	{
		PrintNetworkManagerState();
		try
		{
			Debug.Log("Host - Creating an allocation.");

			// Important: Once the allocation is created, you have ten seconds to BIND
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers-1); // takes number of connections allowed as argument. you can add a region argument

			joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
			textMeshProUGUI.text = joinCode;
			Debug.Log(joinCode);

			RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

			NetworkManager.Singleton.StartHost();
		} 
		catch (RelayServiceException e)
		{
			Debug.LogError(e.Message);
		}
	}

	public async void JoinRelay(string joinCode)
	{
		PrintNetworkManagerState();
		try
		{
			Debug.Log("Joining Relay with " + joinCode);
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

			RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
			NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

			NetworkManager.Singleton.StartClient();
		}
		catch (RelayServiceException e)
		{
			Debug.LogError(e.Message);
		}
	}
	private void PrintNetworkManagerState()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Current NetworkManager state: Host");
        }
        else if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Current NetworkManager state: Server");
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Current NetworkManager state: Client");
        }
        else
        {
            Debug.Log("Current NetworkManager state: Inactive");
        }
    }
}
*/