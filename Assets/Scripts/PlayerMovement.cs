using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float stoppingDistance = 0.1f;

	private bool isInDirectMode = false; // TODO consider making static later

	private Transform mainCamera;
	private ThirdPersonCharacter m_Character;
    private CameraRaycaster cameraRaycaster;
    private Vector3 currentClickTarget;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
		mainCamera = Camera.main.transform;
    }

    private void FixedUpdate()
	{
		if(Input.GetKeyDown(KeyCode.G)) // TODO allow player to remap later
		{
			isInDirectMode = !isInDirectMode;
			currentClickTarget = transform.position;
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
			currentClickTarget = cameraRaycaster.hit.point;
		}

		var distanceToTarget = Vector3.Distance(transform.position, currentClickTarget);

		if (distanceToTarget >= stoppingDistance)
		{
			m_Character.Move(currentClickTarget - transform.position, false, false);
		}
		else
		{
			m_Character.Move(Vector3.zero, false, false);
		}
	}
}

