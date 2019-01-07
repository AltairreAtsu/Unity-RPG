﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

using RPG.Core;

namespace RPG.Characters { 

	public class Enemy : MonoBehaviour, IDamagable
	{
		[SerializeField] private float maxHealthPoints = 10f;
		[SerializeField] private float currentHealth = 10f;

		private AICharacterControl aiCharacter;
		[SerializeField] private float attackRadius = 10f;
		[SerializeField] private float chaseRadius = 20f;
		[Space]
		[SerializeField] private float delayBetweenShots = 0.5f;
		[SerializeField] private float damagePerShot = 5f;
		[SerializeField] private GameObject projectileSocket;
		[SerializeField] private GameObject projectileToUse;
		private Coroutine projectileSpawningCoroutine;
		private bool isAttacking = false;
		[SerializeField] private Vector3 aimOffset = new Vector3(0, 1, 0);

		private Player player;

		public float healthAsPercentage
		{
			get
			{
				return currentHealth / maxHealthPoints;
			}
		}

		public void TakeDamage(float damage)
		{
			currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealthPoints);
			if (currentHealth <= 0) { Destroy(gameObject); }
		}

		public void Heal(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealthPoints);
		}

		private void Start()
		{
			aiCharacter = GetComponent<AICharacterControl>();

			player= GameObject.FindObjectOfType<Player>();
		}

		private void Update()
		{
			float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
			if (player.IsAlive() && distanceToPlayer <= attackRadius && !isAttacking)
			{
				projectileSpawningCoroutine = StartCoroutine(SpawnProjectiles());
				isAttacking = true;
			}
			else if(!player.IsAlive() && isAttacking || distanceToPlayer > attackRadius && isAttacking)
			{
				StopCoroutine(projectileSpawningCoroutine);
				isAttacking = false;
			}

			if (distanceToPlayer <= chaseRadius)
			{
				aiCharacter.SetTarget(player.transform);
			}
			else
			{
				aiCharacter.SetTarget(null);
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