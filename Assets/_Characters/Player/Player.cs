using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] private float maxHealthPoints = 10f;
		[SerializeField] private float currentHealth = 10f;
		[Space]
		[SerializeField] private int enemyLayer = 9;
		[SerializeField] private float damagePerShot = 3f;
		[SerializeField] private float damageDelay = 0.5f;
		[SerializeField] private float attackRange = 0.5f;


		[SerializeField] private Weapon heldWeapon;

		private float lastDamageTime = 0f;

		public float healthAsPercentage { get { return currentHealth / maxHealthPoints; } }

		private void Start()
		{
			Camera.main.GetComponent<CameraRaycaster>().notifyMouseClickObservers += OnMouseClick;

			PutWeaponInHand();
		}

		private void PutWeaponInHand()
		{
			var weaponSocket = RequestDominantHand();

			var weapon = Instantiate(heldWeapon.GetWeaponPrefab(), weaponSocket);
			weapon.transform.localPosition = heldWeapon.getWeaponGrip().transform.position;
			weapon.transform.localRotation = heldWeapon.getWeaponGrip().transform.rotation;
		}

		private Transform RequestDominantHand()
		{
			var dominantHands = GetComponentsInChildren<DominantHand>();
			int numberOfDominantHands = dominantHands.Length;
			Assert.IsFalse(numberOfDominantHands <= 0, "No DominantHand Found on Player, please add the component to the hand transform!");
			Assert.IsFalse(numberOfDominantHands > 1, "Too many DominantHand scripts detected! Please ensure there is only one present on the player object!");
			return dominantHands[0].transform;
		}

		public void TakeDamage(float damage)
		{
			currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealthPoints);
		}

		private void OnMouseClick(RaycastHit hit, int layerHit)
		{
			if (layerHit == enemyLayer)
			{
				var damable = hit.collider.GetComponent<IDamagable>();
				var distanceToPlayer = Vector3.Distance(transform.position, hit.collider.transform.position);

				if ((damable != null) && (Time.time - lastDamageTime > damageDelay) && (distanceToPlayer <= attackRange))
				{
					damable.TakeDamage(damagePerShot);
					lastDamageTime = Time.time;
					//print(string.Format("Player dealing {0} damage!", damagePerShot.ToString()));
				}
			}
		}
	}
}