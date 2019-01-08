using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Weapons;

namespace RPG.Characters { 

	public class EnemyAI : MonoBehaviour
	{
		[Header("Chase Settings")]
		[SerializeField] private float chaseRadius = 20f;


		private Character character;
		private Coroutine projectileSpawningCoroutine;
		private Player player;
		private WeaponSystem weaponSystem;

		private float currentWeaponRange;


		public Health Health { get; private set; }

		private void Start()
		{
			character = GetComponent<Character>();
			Health = GetComponent<Health>();
			Health.onDeathListeners += OnDeath;
			player = FindObjectOfType<Player>();
			weaponSystem = GetComponent<WeaponSystem>();
			currentWeaponRange = weaponSystem.CurrentWepaon.GetAttackRange();
		}

		private void OnDeath(float deathDelay)
		{
			enabled = false;
			weaponSystem.StopAttacking();
			var rigidBody = GetComponent<Rigidbody>();
			rigidBody.useGravity = false;
			rigidBody.velocity = Vector3.zero;
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
			GetComponent<CapsuleCollider>().enabled = false;
			if(projectileSpawningCoroutine != null)
			{
				StopCoroutine(projectileSpawningCoroutine);
			}
		}

		private void Update()
		{
			if (!Health.Alive) { return; }

			float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			UpdateAttackState(distanceToPlayer);
			UpdateChaseState(distanceToPlayer);
		}

		private void UpdateChaseState(float distanceToPlayer)
		{
			if (distanceToPlayer <= chaseRadius)
			{
				character.SetTarget(player.transform);
			}
			else
			{
				character.SetTarget(null);
			}
		}

		private void UpdateAttackState(float distanceToPlayer)
		{
			var currentlyAttacking = weaponSystem.CurrentlyAttacking;
			var playerAliveAndInRange = player.Health.Alive && distanceToPlayer <= currentWeaponRange;
			var PlayerOutofRangeAndEnemyAttacking = distanceToPlayer > currentWeaponRange && currentlyAttacking;

			if (playerAliveAndInRange && !currentlyAttacking)
			{
				weaponSystem.StartAttacking(player.Health);
			}
			else if (!player.Health.Alive && currentlyAttacking || PlayerOutofRangeAndEnemyAttacking)
			{
				weaponSystem.StopAttacking();
			}
		}

		public void OnDrawGizmos()
		{
			currentWeaponRange = GetComponent<WeaponSystem>().CurrentWepaon.GetAttackRange();
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, currentWeaponRange);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRadius);
		}

		public Transform GetTransform()
		{
			return transform;
		}
	}
}