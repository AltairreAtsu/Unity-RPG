using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters { 

	public class Enemy : MonoBehaviour
	{
		[Header("Chase Settings")]
		[SerializeField] private float attackRadius = 10f;
		[SerializeField] private float chaseRadius = 20f;
		[Header("Projectile Settings")]
		[SerializeField] private float delayBetweenShots = 0.5f;
		[SerializeField] private float damagePerShot = 5f;
		[SerializeField] private GameObject projectileSocket;
		[SerializeField] private GameObject projectileToUse;
		[SerializeField] private Vector3 aimOffset = new Vector3(0, 1, 0);

		private CharacterMovement locomotion;
		private Coroutine projectileSpawningCoroutine;
		private Player player;

		private bool isAttacking = false;

		public Health Health { get; private set; }

		private void Start()
		{
			player= GameObject.FindObjectOfType<Player>();
			Health = GetComponent<Health>();
			Health.onDeathListeners += OnDeath;
			locomotion = GetComponent<CharacterMovement>();
		}

		private void OnDeath(float deathDelay)
		{
			enabled = false;
			locomotion.enabled = false;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
			GetComponent<CapsuleCollider>().enabled = false;
			if(projectileSpawningCoroutine != null)
			{
				StopCoroutine(projectileSpawningCoroutine);
			}
		}

		private void Update()
		{
			float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			if (player.Health.Alive && distanceToPlayer <= attackRadius && !isAttacking)
			{
				projectileSpawningCoroutine = StartCoroutine(SpawnProjectiles());
				isAttacking = true;
			}
			else if(!player.Health.Alive && isAttacking || distanceToPlayer > attackRadius && isAttacking)
			{
				StopCoroutine(projectileSpawningCoroutine);
				isAttacking = false;
			}

			if (distanceToPlayer <= chaseRadius)
			{
				locomotion.SetTarget(player.transform);
			}
			else
			{
				locomotion.SetTarget(null);
			}	
		}

		private IEnumerator SpawnProjectiles()
		{
			while (true)
			{
				var projectile = Instantiate(projectileToUse, projectileSocket.transform.position, Quaternion.identity)
				.GetComponent<Projectile>();
				projectile.SetShooter(gameObject);
				projectile.SetDamage(damagePerShot);
				projectile.Launch(player.transform.position + aimOffset);

				yield return new WaitForSeconds(delayBetweenShots);
			}
		}

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, attackRadius);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, chaseRadius);
		}

		public Transform GetTransform()
		{
			return transform;
		}
	}
}