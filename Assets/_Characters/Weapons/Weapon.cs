using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons {
	public enum Hand { OffHand, DominantHand }

	[CreateAssetMenu(menuName ="RPG/Weapon")]
	public class Weapon : ScriptableObject
	{
		[SerializeField] private GameObject weaponPrefab = null;
		[SerializeField] private Transform weaponGrip = null;
		[SerializeField] private Hand weaponHand;
		[SerializeField] private AnimationClip[] attackAnimations = null;
		[SerializeField] private float weaponDamage = 1f;
		[SerializeField] private float attackCooldown = 1f;
		[SerializeField] private float attackRange = 3f;
		[SerializeField] private float attackSpeedMultiplier = 1f;

		public Transform getWeaponGrip()
		{
			return weaponGrip;
		}

		public GameObject GetWeaponPrefab()
		{
			return weaponPrefab;
		}

		public AnimationClip GetAnimation()
		{
			var attackAnimation = attackAnimations[Random.Range(0, attackAnimations.Length - 1)];

			return RemoveAnimationEvents(attackAnimation);
		}

		public float GetAttackCooldown()
		{
			return attackCooldown;
		}

		public float GetAttackRange()
		{
			return attackRange;
		}

		public Hand GetHand()
		{
			return weaponHand;
		}

		public float GetAttackSpeedMultiplier()
		{
			return attackSpeedMultiplier;
		}

		public float GetWeaponDamage()
		{
			return weaponDamage;
		}

		private AnimationClip RemoveAnimationEvents(AnimationClip attackAnimation)
		{
			attackAnimation.events = new AnimationEvent[0];
			return attackAnimation;
		}
	}
}