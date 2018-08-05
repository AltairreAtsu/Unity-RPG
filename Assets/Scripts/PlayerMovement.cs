using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

[RequireComponent(typeof (ThirdPersonCharacter))]
public class PlayerMovement : MonoBehaviour
{
	[SerializeField] float stoppingDistance = 0.1f;

	ThirdPersonCharacter m_Character;   // A reference to the ThirdPersonCharacter on the object
    CameraRaycaster cameraRaycaster;
    Vector3 currentClickTarget;

    private void Start()
    {
        cameraRaycaster = Camera.main.GetComponent<CameraRaycaster>();
        m_Character = GetComponent<ThirdPersonCharacter>();
        currentClickTarget = transform.position;
    }

    private void FixedUpdate()
    {
        if ( (Input.GetMouseButton(0)) && (cameraRaycaster.layerHit == Utils.Layer.Walkable) )
        {
			currentClickTarget = cameraRaycaster.hit.point;  // So not set in default case
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

