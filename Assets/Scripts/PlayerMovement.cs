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

	private Transform mainCamera;
	private ThirdPersonCharacter m_Character;
    private CameraRaycaster cameraRaycaster;
    private Vector3 currentDestination, clickTarget;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentDestination = transform.position;
		mainCamera = Camera.main.transform;
    }

    private void FixedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.G)) // TODO allow player to remap later
		{
			isInDirectMode = !isInDirectMode;
			currentDestination = transform.position;
		}

		if (isInDirectMode)
		{
			ProcessDirectMovement();
		}
		else
		{
			ProcessClickToMove();
		}
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

	private void ProcessClickToMove()
	{
		if ((Input.GetMouseButton(0)) && (cameraRaycaster.layerHit == Utils.Layer.Walkable))
		{
			clickTarget = cameraRaycaster.hit.point;
			currentDestination = ShortDestination(clickTarget, walkStopRadius);
		}
		else if ((Input.GetMouseButton(0)) && (cameraRaycaster.layerHit == Utils.Layer.Enemy))
		{
			clickTarget = cameraRaycaster.hit.point;
			currentDestination = ShortDestination(clickTarget, attackMoveStopRadius);
		}

		WalkToDestination();
	}

	private void WalkToDestination()
	{
		var distanceToTarget = Vector3.Distance(transform.position, currentDestination);

		if (distanceToTarget >= postionalMarginOfError)
		{
			m_Character.Move(currentDestination - transform.position, false, false);
		}
		else
		{
			m_Character.Move(Vector3.zero, false, false);
		}
	}

	private Vector3 ShortDestination(Vector3 destination, float shortening)
	{
		Vector3 reductionVector = (destination - transform.position).normalized * shortening;
		return destination - reductionVector;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(clickTarget, 0.1f);
		Gizmos.DrawSphere(currentDestination, 0.05f);
		Gizmos.DrawLine(transform.position, currentDestination);
	}
}

