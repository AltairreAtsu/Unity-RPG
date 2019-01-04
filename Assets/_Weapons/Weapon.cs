using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons {
	public enum Hand { OffHand, DominantHand }

	[CreateAssetMenu(menuName ="RPG/Weapon")]
	public class Weapon : ScriptableObject
	{
		[SerializeField] private GameObject weaponPrefab;
		[SerializeField] private Transform weaponGrip;
		[SerializeField] private Hand weaponHand;
		[SerializeField] private AnimationClip attackAnimation;
		[SerializeField] private float attackCooldown;
		[SerializeField] private float attackRange;

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
			// Prevents errors from being thrown due to null animation event recievers.
			RemoveAnimationEvents();
			return attackAnimation;
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

		private void RemoveAnimationEvents()
		{
			attackAnimation.events = new AnimationEvent[0];
		}
	}
}