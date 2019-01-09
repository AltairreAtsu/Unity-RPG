using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class PlayerInput : MonoBehaviour
	{
		private Character character;
		private CameraRaycaster cameraRaycaster;
		private EnemyAI currentTarget;
		private Player player;
		private SpecialAbilities specialAbilities;
		private Transform currentWalkTarget;

		private Coroutine movementCoroutine;

		private delegate void MovementCallback();

		private void Start()
		{
			character = GetComponent<Character>();
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
					specialAbilities.TryPerformSpecialAbility(i);
				}
			}
			if (Input.GetKeyDown("0"))
			{
				specialAbilities.TryPerformSpecialAbility(10);
			}
		}

		private void OnMouseOverWalkable(Vector3 point)
		{
			if (Input.GetMouseButton(0))
			{
				currentWalkTarget.transform.position = point;
				character.SetTarget(currentWalkTarget);
			}
		}

		private void OnMouseOverEnemy(EnemyAI enemy)
		{
			if (Input.GetMouseButtonDown(0))
			{
				if (!player.WeaponSystem.IsInRange(enemy.transform.position))
				{
					
					if(movementCoroutine == null)
					{
						currentTarget = enemy;
						StartCoroutine(MoveInRange(enemy.transform, () => MoveToAttackEnemyCallback()));
					}
				}
				else
				{
					var health = enemy.GetComponent<Health>();
					if (health != null) player.WeaponSystem.TryAttack(health);
				}
			}
			if (Input.GetMouseButtonDown(1))
			{
				if (movementCoroutine == null)
				{
					currentTarget = enemy;
					StartCoroutine(MoveInRange(enemy.transform, () => MoveToPowerAttackEnemyCallback()));
				}
				else
				{
					specialAbilities.TryPerformPowerAttack(enemy.Health);
				}
			}
		}

		private void MoveToAttackEnemyCallback()
		{
			if (currentTarget != null && currentTarget.Health.Alive)
			{
				player.WeaponSystem.TryAttack(currentTarget.Health);
			}
		}
		private void MoveToPowerAttackEnemyCallback()
		{
			if (currentTarget != null && currentTarget.Health.Alive)
			{
				specialAbilities.TryPerformPowerAttack(currentTarget.Health);
			}
		}

		private IEnumerator MoveInRange(Transform target, MovementCallback callback)
		{
			character.SetTarget(target);
			while (true)
			{
				if (player.WeaponSystem.IsInRange(target.position))
				{
					callback();
					yield break;
				}
				yield return new WaitForEndOfFrame();
			}
		}
	}

}
