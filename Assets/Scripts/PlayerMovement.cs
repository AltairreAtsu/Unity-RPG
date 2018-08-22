using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	private bool isInDirectMode = false; // TODO consider making static later

	private AICharacterControl aiCharacter = null;
	private Transform mainCamera = null;
	private ThirdPersonCharacter thirdPersonCharacter = null;
    private CameraRaycaster cameraRaycaster = null;

	private Transform currentWalkTarget;

    private void Start()
    {
		GetDependencies();

		cameraRaycaster.notifyMouseClickObservers += OnMouseClick;

		CreateWalkTarget();
    }

	private void GetDependencies()
	{
		aiCharacter = GetComponent<AICharacterControl>();
		thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();

		mainCamera = Camera.main.transform;
		cameraRaycaster = mainCamera.GetComponent<CameraRaycaster>();
	}

	private void CreateWalkTarget()
	{
		var walkTargetObject = new GameObject("CurrentWalkTarget");
		currentWalkTarget = walkTargetObject.transform;
	}

	private void ProcessDirectMovement()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// calculate camera relative direction to move:
		Vector3 m_CamForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 m_Move = v * m_CamForward + h * mainCamera.right;

		thirdPersonCharacter.Move(m_Move, false, false);
	}

	private void OnMouseClick(RaycastHit hit, int layerHit)
	{
		if (layerHit == 10)
		{
			aiCharacter.SetTarget(hit.transform);
		}
		else if (layerHit == 9)
		{
			currentWalkTarget.position = hit.point;
			aiCharacter.SetTarget(currentWalkTarget);
		}
	}
}

