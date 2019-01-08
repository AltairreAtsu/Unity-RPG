using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerMouseControl : MonoBehaviour
	{
		private CharacterMovement locomotion;
		private CameraRaycaster cameraRaycaster;
		private Player player;
		private Transform currentWalkTarget;

		private void Start()
		{
			locomotion = GetComponent<CharacterMovement>();
			player = GetComponent<Player>();
			cameraRaycaster = Camera.main.transform.GetComponent<CameraRaycaster>();

			cameraRaycaster.onMouseOverWalkable += OnMouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			CreateWalkTarget();
		}

		public void UnSubscribeFromEvents()
		{
			cameraRaycaster.onMouseOverWalkable -= OnMouseOverWalkable;
			cameraRaycaster.onMouseOverEnemy -= OnMouseOverEnemy;
		}

		private void CreateWalkTarget()
		{
			var walkTargetObject = new GameObject("CurrentWalkTarget");
			currentWalkTarget = walkTargetObject.transform;
		}

		private void OnMouseOverWalkable(Vector3 point)
		{
			if (Input.GetMouseButton(0))
			{
				currentWalkTarget.transform.position = point;
				locomotion.SetTarget(currentWalkTarget);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!player.InRange(enemy.transform.position))
				{
					locomotion.SetTarget(enemy.transform);
				}
				else
				{
					player.TryAttack(enemy);
				}
			}
		}
	}

}
