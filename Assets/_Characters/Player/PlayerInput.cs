﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerInput : MonoBehaviour
	{
		private CharacterMovement locomotion;
		private CameraRaycaster cameraRaycaster;
		private Player player;
		private SpecialAbilities specialAbilities;
		private Transform currentWalkTarget;

		private void Start()
		{
			locomotion = GetComponent<CharacterMovement>();
			cameraRaycaster = Camera.main.transform.GetComponent<CameraRaycaster>();
			player = GetComponent<Player>();
			specialAbilities = GetComponent<SpecialAbilities>();

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

		private void Update()
		{
			ScanForAbilityInput();
		}

		private void ScanForAbilityInput()
		{
			for (int i = 1; i < 10; i++)
			{
				if (Input.GetKeyDown(i.ToString())) 
				{
					specialAbilities.TryPerformSpecialAbility(i-1);
				}
			}
			if (Input.GetKeyDown("0"))
			{
				specialAbilities.TryPerformSpecialAbility(9);
			}
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
			if (Input.GetMouseButtonDown(1))
			{
				specialAbilities.TryPerformPowerAttack(enemy.Health);
			}
		}
	}

}