﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;


namespace RPG.Characters
{
	public class Player : MonoBehaviour
	{
		[SerializeField] private float baseDamage = 3f;
		[SerializeField] private AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] private Weapon heldWeapon = null;

		[Header("Pickup Variables")]
		[SerializeField] private float pickupRange = 3f;
		[Header("Critical Hit Variables")]
		[SerializeField] [Range(0f, 1.0f)] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.5f;
		[SerializeField] GameObject criticalHitVFX = null;

		private Animator animator;
		private Coroutine deathCorotune;
		private GameObject weaponObject;

		private float lastDamageTime = 0f;

		public Health Health { get; private set; }
		public float BaseDamage { get { return baseDamage; } }

		private void Start()
		{
			animator = GetComponent<Animator>();

			Health = GetComponent<Health>();
			Health.onDeathListeners += delegate(float deathDelay) { StartCoroutine(OnPlayerDeath(deathDelay)); };

			PutWeaponInHand(heldWeapon);
			animator.runtimeAnimatorController = animatorOverrideController;

			var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
			cameraRaycaster.onMouseOverPickup += OnMouseOverPickup;
		}

		private void Update()
		{
			if (!Health.Alive) { return; }
		}

		private void OnMouseOverPickup(WeaponPickupPoint pickup)
		{
			if( Vector3.Distance(transform.position, pickup.transform.position) <= pickupRange 
				&& Input.GetMouseButtonDown(0))
			{
				// Move towards weapon if not close enough?
				// Play Sound
				// Trigger Animation
				PutWeaponInHand(pickup.GetWeapon());
			}
		}

		private void PutWeaponInHand(Weapon weapon)
		{
			if(weaponObject != null)
			{
				Destroy(weaponObject);
			}
			Transform weaponSocket = null;
			switch (weapon.GetHand()){
				case Hand.OffHand:
					weaponSocket = RequestOffHand();
					break;
				case Hand.DominantHand:
					weaponSocket = RequestDominantHand();
					break;
			}

			weaponObject = Instantiate(weapon.GetWeaponPrefab(), weaponSocket);
			weaponObject.transform.localPosition = weapon.getWeaponGrip().transform.position;
			weaponObject.transform.localRotation = weapon.getWeaponGrip().transform.rotation;

			animator.SetFloat("AttackAnimationSpeedMultiplier", weapon.GetAttackSpeedMultiplier());
			heldWeapon = weapon;
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

		private IEnumerator OnPlayerDeath(float deathDelay)
		{
			DisablePlayerInput();
			yield return new WaitForSecondsRealtime(deathDelay);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void DisablePlayerInput()
		{
			var playerMouseInput = GetComponent<PlayerInput>();
			playerMouseInput.UnSubscribeFromEvents();
			playerMouseInput.enabled = false;
		}

		public void TryAttack (Enemy enemy)
		{
			var target = enemy.GetComponent<Health>();
			if (target == null) { return; }

			if (CanAttack(enemy.transform.position))
			{
				Attack(target, enemy.gameObject);
			}
		}

		private void Attack(Health target, GameObject targetObject)
		{
			transform.LookAt(targetObject.transform);
			animator.SetTrigger("Attack");
			target.TakeDamage(CalculateDamage());
			lastDamageTime = Time.time;
			animatorOverrideController["DEFUALT_ATTACK"] = heldWeapon.GetAnimation();
		}


		private float CalculateDamage()
		{
			if (Random.value <= criticalHitChance)
			{
				PerformCriticalHit();
				return (baseDamage + heldWeapon.GetWeaponDamage()) * 3; 
			}
			else
			{
				return baseDamage + heldWeapon.GetWeaponDamage();
			}
		}

		private void PerformCriticalHit()
		{
			var vfxSystem = Instantiate(criticalHitVFX, transform).GetComponent<CompoundParticleSystem>();
			vfxSystem.InitAndPlay(selfDestruct: true);
		}

		private bool CanAttack(Vector3 position)
		{	
			var AttackCooldown = (Time.time - lastDamageTime < heldWeapon.GetAttackCooldown());

			return (!AttackCooldown) && (InRange(position));
		}

		public bool InRange(Vector3 position)
		{
			var distanceToPlayer = Vector3.Distance(transform.position, position);
			return distanceToPlayer <= heldWeapon.GetAttackRange();
		}

		public Transform GetTransform()
		{
			return transform;
		}
	}
}