using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Weapons
{
	[ExecuteInEditMode]
	public class WeaponPickupPoint : MonoBehaviour
	{
		[SerializeField] private Weapon weaponConfig;
		[SerializeField] private AudioClip pcikupSound;

		private Weapon cachedConfig;

		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if (!Application.isPlaying)
			{
				DoEditModeUpdate();
			}

		}

		private void DoEditModeUpdate()
		{
			if(cachedConfig == null)
			{
				cachedConfig = weaponConfig;
			}

			if(cachedConfig != weaponConfig)
			{
				UpdateWeapon();
			}
		}

		private void UpdateWeapon()
		{
			cachedConfig = weaponConfig;
			DestroyAllChildren();
			var weaponObject = Instantiate(weaponConfig.GetWeaponPrefab(), transform);
			weaponObject.transform.localPosition = Vector3.zero;
		}

		private void DestroyAllChildren()
		{
			if (transform.childCount == 0) { return; }
			foreach (Transform child in transform)
			{
				DestroyImmediate(child.gameObject);
			}
		}

		public Weapon GetWeapon()
		{
			return weaponConfig;
		}
	}

}
