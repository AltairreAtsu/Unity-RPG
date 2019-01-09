using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Weapons;

namespace RPG.Characters { 

	public class EnemyAI : MonoBehaviour
	{
		[Header("Chase Settings")]
		[SerializeField] private float chaseRadius = 20f;
		[SerializeField] private WaypointPath patrolPath;

		enum States { Idle, Patroling, Chasing, Attacking }
		States state = States.Idle;

		private Character character;
		private Coroutine patrolCoroutine;
		private Player player;
		private WaypointIterator iterator;
		private WeaponSystem weaponSystem;

		private Vector3 originalPosition;

		private float currentWeaponRange;
		private bool OutOfCombat { get { return state == States.Idle || state == States.Patroling; } }


		public Health Health { get; private set; }

		private void Start()
		{
			character = GetComponent<Character>();
			Health = GetComponent<Health>();
			Health.onDeathListeners += OnDeath;
			player = FindObjectOfType<Player>();
			weaponSystem = GetComponent<WeaponSystem>();
			currentWeaponRange = weaponSystem.CurrentWepaon.GetAttackRange();
			originalPosition = transform.position;

			if (patrolPath)
			{
				iterator = patrolPath.GetNewIterator();
				state = States.Patroling;
				character.SetTarget(patrolPath.GetFirstWaypoint());
				Patrol();
			}
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
		}

		private void Update()
		{
			if (!Health.Alive) { return; }

			float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			var playerInRange = weaponSystem.IsInRange(player.transform.position);

			if (OutOfCombat && distanceToPlayer <= chaseRadius)
			{
				if(patrolCoroutine != null)
				{
					StopCoroutine(patrolCoroutine);
					patrolCoroutine = null;
				}
				state = States.Chasing;
				character.SetTarget(player.transform);
				return;
			}
			if (state == States.Chasing && playerInRange && player.Health.Alive )
			{
				state = States.Attacking;
				weaponSystem.StartAttacking(player.Health);
				return;
			}
			if (state == States.Attacking && !playerInRange 
				|| state == States.Attacking && !player.Health.Alive)
			{
				state = States.Chasing;
				weaponSystem.StopAttacking();
				return;
			}
			if (state == States.Chasing && distanceToPlayer > chaseRadius )
			{
				TransitionOutOfCombat();
				return;
			}
			if (state == States.Patroling)
			{
				Patrol();
				return;
			}
		}

		private void Patrol()
		{
			if (patrolCoroutine == null)
			{
				patrolCoroutine = StartCoroutine(DoPatrol());
			}
		}

		private IEnumerator DoPatrol()
		{
			while (true)
			{
				if (patrolPath.AtWaypoint(iterator, transform.position))
				{
					if (iterator.pause > 0)
					{
						yield return new WaitForSeconds(iterator.pause);
						iterator.pause = 0f;
					}
					character.SetTarget(patrolPath.GetNextWaypoint(iterator));
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private void TransitionOutOfCombat()
		{
			weaponSystem.StopAttacking();
			if (patrolPath)
			{
				state = States.Patroling;
				Patrol();
			}
			else
			{
				state = States.Idle;
				character.SetTarget(originalPosition);
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