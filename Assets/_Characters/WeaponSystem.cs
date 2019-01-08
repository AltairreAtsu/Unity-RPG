using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using RPG.Characters;
using RPG.Core;

namespace RPG.Weapons
{
	public class WeaponSystem : MonoBehaviour
	{
		[SerializeField] private AnimatorOverrideController animatorOverrideController;
		[SerializeField] private WeaponConfig heldWeapon ;
		[SerializeField] private float baseDamage = 3f;
		[Header("Critical Hit Variables")]
		[SerializeField] [Range(0f, 1.0f)] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.5f;
		[SerializeField] GameObject criticalHitVFX ;

		private Animator animator;
		private Coroutine attackingCoroutine;
		private GameObject weaponObject;
		private float lastDamageTime = 0f;

		public WeaponConfig CurrentWepaon { get { return heldWeapon; } }
		public bool CurrentlyAttacking { get { return attackingCoroutine != null; } }
		public float BaseDamage { get { return baseDamage; } }

		private void Start()
		{
			animator = GetComponent<Animator>();
			PutWeaponInHand(heldWeapon);
		}

		private void PutWeaponInHand(WeaponConfig weapon)
		{
			if (weaponObject != null)
			{
				Destroy(weaponObject);
			}
			Transform weaponSocket = null;
			switch (weapon.GetHand())
			{
				case Hand.OffHand:
					weaponSocket = RequestOffHand();
					break;
				case Hand.DominantHand:
					weaponSocket = RequestDominantHand();
					break;
			}

			weaponObject = Instantiate(weapon.GetWeaponPrefab(), weaponSocket);
			var weaponGrip = weapon.getWeaponGrip();
			weaponObject.transform.localPosition = weaponGrip.transform.position;
			weaponObject.transform.localRotation = weaponGrip.transform.rotation;
			weaponObject.transform.localScale = weaponGrip.transform.localScale;

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

		public void TryAttack(Health health)
		{

			if (CanAttack(health.transform.position))
			{
				Attack(health, health.gameObject);
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
				return (baseDamage + heldWeapon.GetWeaponDamage()) * criticalHitMultiplier;
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

		public void StartAttacking(Health target)
		{
			if(attackingCoroutine != null)
			{
				StopCoroutine(attackingCoroutine);
			}
			attackingCoroutine = StartCoroutine(RepeatAttack(target));
		}

		public void StopAttacking()
		{
			if(attackingCoroutine != null)
			{
				StopCoroutine(attackingCoroutine);
				attackingCoroutine = null;
			}
		}

		private IEnumerator RepeatAttack(Health target)
		{
			while (target.Alive)
			{
				TryAttack(target);
				yield return new WaitForSeconds(heldWeapon.GetAttackCooldown() + 0.1f);
			}
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
	}
}