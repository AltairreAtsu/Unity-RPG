using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;
using System;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] private float maxHealthPoints = 10f;
		[SerializeField] private float currentHealth = 10f;
		[Space]
		[SerializeField] private int enemyLayer = 9;
		[SerializeField] private float damagePerShot = 3f;

		[SerializeField] private Weapon heldWeapon;

		[SerializeField] private AnimatorOverrideController animatorOverrideController;

		private Animator animator;

		private float lastDamageTime = 0f;
		public float healthAsPercentage { get { return currentHealth / maxHealthPoints; } }

		private void Start()
		{
			Camera.main.GetComponent<CameraRaycaster>().notifyMouseClickObservers += OnMouseClick;
			animator = GetComponent<Animator>();

			PutWeaponInHand();
			SetupRuntimeAnimator();
		}

		private void SetupRuntimeAnimator()
		{
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["DEFUALT_ATTACK"] = heldWeapon.GetAnimation();
		}

		private void PutWeaponInHand()
		{
			Transform weaponSocket = null;
			switch (heldWeapon.GetHand()){
				case Hand.OffHand:
					weaponSocket = RequestOffHand();
					break;
				case Hand.DominantHand:
					weaponSocket = RequestDominantHand();
					break;
			}

			var weapon = Instantiate(heldWeapon.GetWeaponPrefab(), weaponSocket);
			weapon.transform.localPosition = heldWeapon.getWeaponGrip().transform.position;
			weapon.transform.localRotation = heldWeapon.getWeaponGrip().transform.rotation;
		}

		private HandIndicator[] GetHands()
		{
			var hands = GetComponentsInChildren<HandIndicator>();
			int numberOfHands = hands.Length;
			Assert.IsFalse(numberOfHands <= 0, "No HandIndicators Found on Player, please add the component to the hand transforms!");
			Assert.IsFalse(numberOfHands > 2, "Too many HandIndicators scripts detected! Please ensure there are only two present on the player object!");
			return hands;
		}

		private Transform RequestOffHand()
		{
			var hands = GetHands();
			foreach (HandIndicator hand in hands)
			{
				if (!hand.Dominant)
				{
					return hand.transform;
				}
			}
			return null;
		}

		private Transform RequestDominantHand()
		{
			var hands = GetHands();
			foreach (HandIndicator hand in hands)
			{
				if (hand.Dominant)
				{
					return hand.transform;
				}
			}
			return null;
		}

		public void TakeDamage(float damage)
		{
			currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealthPoints);
		}

		private void OnMouseClick(RaycastHit hit, int layerHit)
		{
			if (layerHit != enemyLayer) { return; }

			var target = hit.collider.GetComponent<IDamagable>();
			if( target == null ) { return; }

			if (CanAttack(hit))
			{
				Attack(target, hit.collider.gameObject);
			}
		}

		private void Attack(IDamagable target, GameObject targetObject)
		{
			transform.LookAt(targetObject.transform);

			animator.SetTrigger("Attack");
			target.TakeDamage(damagePerShot);
			lastDamageTime = Time.time;
		}

		private bool CanAttack(RaycastHit hit)
		{	
			var AttackCooldown = (Time.time - lastDamageTime < heldWeapon.GetAttackCooldown());

			return (!AttackCooldown) && (InRange(hit));
		}

		public bool InRange(RaycastHit hit)
		{
			var distanceToPlayer = Vector3.Distance(transform.position, hit.transform.position);
			return distanceToPlayer <= heldWeapon.GetAttackRange();
		}
	}
}