using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction.Input;

public class NetworkPlayer : NetworkBehaviour
{
	//First define some global variables in order to speed up the Update() function
	GameObject myXRRig;
	OVRCameraRigRef RigRef;                 //Script with transform parameteres for camera and controllers.
	Transform myXRLC, myXRRC, myXRCam;                  //positions and rotations of controllers and camera
	Transform avHead, avLeft, avRight, avBody;          //avatars moving parts 

	//some fine tuning parameters if needed
	[SerializeField]
	private Vector3 avatarLeftPositionOffset, avatarRightPositionOffset;
	[SerializeField]
	private Quaternion avatarLeftRotationOffset, avatarRightRotationOffset;
	[SerializeField]
	private Vector3 avatarHeadPositionOffset;
	[SerializeField]
	private Quaternion avatarHeadRotationOffset;
	[SerializeField]
	private Vector3 avatarBodyPositionOffset;

	// Start is called before the first frame update
	public override void OnNetworkSpawn()
	{
		var myID = transform.GetComponent<NetworkObject>().NetworkObjectId;
		if (IsOwnedByServer)
			transform.name = "Host:" + myID;    //this must be the host
		else
			transform.name = "Client:" + myID; //this must be the client 

		if (!IsOwner) return;

		myXRRig = GameObject.Find("OVRInteraction");
		if (myXRRig) Debug.Log("Found OVRCameraRig");
		else Debug.Log("Could not find OVRCameraRig!");

		//pointers to the XR RIg
		RigRef = myXRRig.GetComponent<OVRCameraRigRef>();
		myXRLC = RigRef.LeftController;
		myXRRC = RigRef.RightController;
		myXRCam = RigRef.CameraRig.centerEyeAnchor.transform;

		//pointers to the avatar
		avLeft = transform.Find("Left Hand");
		avRight = transform.Find("Right Hand");
		avHead = transform.Find("Head");
		avBody = transform.Find("Body");
	}

	void Update()
	{
		if (!IsOwner) return;
		if (!myXRRig) return;

		if (avLeft)
		{
			avLeft.rotation = myXRLC.rotation * avatarLeftRotationOffset;
			avLeft.position = myXRLC.position + avatarLeftPositionOffset.x * myXRLC.transform.right + avatarLeftPositionOffset.y * myXRLC.transform.up + avatarLeftPositionOffset.z * myXRLC.transform.forward;
		}

		if (avRight)
		{
			avRight.rotation = myXRRC.rotation * avatarRightRotationOffset;
			avRight.position = myXRRC.position + avatarRightPositionOffset.x * myXRRC.transform.right + avatarRightPositionOffset.y * myXRRC.transform.up + avatarRightPositionOffset.z * myXRRC.transform.forward;
		}

		if (avHead)
		{
			avHead.rotation = myXRCam.rotation/* * avatarHeadRotationOffset*/;
			avHead.position = myXRCam.position + avatarHeadPositionOffset.x * myXRCam.transform.right + avatarHeadPositionOffset.y * myXRCam.transform.up + avatarHeadPositionOffset.z * myXRCam.transform.forward;
		}

		if (avBody)
		{
			avBody.position = avHead.position + avatarBodyPositionOffset;
		}
	}
}
