using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float postionalMarginOfError = 0.2f;
	[SerializeField] float walkStopRadius = 0.2f;
	[SerializeField] float attackMoveStopRadius = 1f;

	private bool isInDirectMode = false; // TODO consider making static later

	private AICharacterControl aiCharacter;
	private Transform mainCamera;
	private ThirdPersonCharacter m_Character;
    private CameraRaycaster cameraRaycaster;

	private Transform currentWalkTarget;

	// TODO Remove(?)
    private Vector3 currentDestination, clickTarget;

    private void Start()
    {
		GetDependencies();

		cameraRaycaster.notifyMouseClickObservers += OnMouseClick;

		CreateWalkTarget();
    }

	private void GetDependencies()
	{
		aiCharacter = GetComponent<AICharacterControl>();
		m_Character = GetComponent<ThirdPersonCharacter>();

		mainCamera = Camera.main.transform;
		cameraRaycaster = mainCamera.GetComponent<CameraRaycaster>();
	}

	private void CreateWalkTarget()
	{
		var walkTargetObject = new GameObject("CurrentWalkTarget");
		currentWalkTarget = walkTargetObject.transform;
		currentDestination = currentWalkTarget.position;
	}

	private void ProcessDirectMovement()
	{
		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		// calculate camera relative direction to move:
		Vector3 m_CamForward = Vector3.Scale(mainCamera.forward, new Vector3(1, 0, 1)).normalized;
		Vector3 m_Move = v * m_CamForward + h * mainCamera.right;

		m_Character.Move(m_Move, false, false);
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

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(clickTarget, 0.1f);
		Gizmos.DrawSphere(currentDestination, 0.05f);
		Gizmos.DrawLine(transform.position, currentDestination);
	}
}

