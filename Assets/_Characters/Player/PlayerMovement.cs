using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

using RPG.CameraUI; // TODO consider re-wiring

namespace RPG.Characters
{
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class PlayerMovement : MonoBehaviour
	{

		private AICharacterControl aiCharacter = null;
		private Transform mainCamera = null;
		private ThirdPersonCharacter thirdPersonCharacter = null;
		private CameraRaycaster cameraRaycaster = null;
		private Player player;

		private Transform currentWalkTarget;

		private void Start()
		{
			GetDependencies();

			cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;

			CreateWalkTarget();
		}

		public void UnSubscribeFromEvents()
		{
			cameraRaycaster.onMouseOverWalkable -= OnMouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy -= OnMouseOverEnemy;
		}

		private void GetDependencies()
		{
			aiCharacter = GetComponent<AICharacterControl>();
			thirdPersonCharacter = GetComponent<ThirdPersonCharacter>();
			player = GetComponent<Player>();

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

			thirdPersonCharacter.Move(m_Move);
		}

		private void OnMouseOverWalkable(Vector3 point)
		{
			if (Input.GetMouseButton(0))
			{
				currentWalkTarget.position = point;
				aiCharacter.SetTarget(currentWalkTarget);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!player.InRange(enemy.transform.position))
				{
					aiCharacter.SetTarget(enemy.transform);
				}
				else
				{
					player.TryAttack(enemy);
				}
			}
		}
	}
}