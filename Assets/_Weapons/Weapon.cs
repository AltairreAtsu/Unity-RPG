using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons { 
	[CreateAssetMenu(menuName ="RPG/Weapon")]
	public class Weapon : ScriptableObject
	{
		[SerializeField] private GameObject weaponPrefab;
		[SerializeField] private Transform weaponGrip;
		[SerializeField] private AnimationClip attackAnimation;

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

		private void RemoveAnimationEvents()
		{
			attackAnimation.events = new AnimationEvent[0];
		}
	}
}