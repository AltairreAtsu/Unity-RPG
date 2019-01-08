using System;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

using RPG.CameraUI; // TODO consider re-wiring
using UnityEngine.AI;

namespace RPG.Characters
{
	[RequireComponent(typeof(ThirdPersonCharacter))]
	public class CharacterMovement : MonoBehaviour
	{
		private Transform mainCamera;
		private ThirdPersonCharacter character;
		private CameraRaycaster cameraRaycaster;
		private Player player;
		private NavMeshAgent agent;
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
			character = GetComponent<ThirdPersonCharacter>();
			player = GetComponent<Player>();
			agent = GetComponent<NavMeshAgent>();
			agent.updatePosition = true;
			agent.updateRotation = false;

			mainCamera = Camera.main.transform;
			cameraRaycaster = mainCamera.GetComponent<CameraRaycaster>();
		}

		private void CreateWalkTarget()
		{
			var walkTargetObject = new GameObject("CurrentWalkTarget");
			currentWalkTarget = walkTargetObject.transform;
		}

		private void Update()
		{
			if (agent.remainingDistance > agent.stoppingDistance)
			{
				character.Move(agent.desiredVelocity);
			}
			else
			{
				character.Move(Vector3.zero);
			}
		}

		private void OnMouseOverWalkable(Vector3 point)
		{
			if (Input.GetMouseButton(0))
			{
				agent.SetDestination(point);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!player.InRange(enemy.transform.position))
				{
					agent.SetDestination(transform.position);
				}
				else
				{
					player.TryAttack(enemy);
				}
			}
		}
	}
}