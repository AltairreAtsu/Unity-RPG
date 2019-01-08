using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
	public class Player : Character
	{
		private Coroutine deathCorotune;

		public WeaponSystem WeaponSystem { get; private set; }
		public Health Health { get; private set; }

		private void Start()
		{
			Health = GetComponent<Health>();
			Health.onDeathListeners += delegate(float deathDelay) { StartCoroutine(OnPlayerDeath(deathDelay-0.1f)); };

			WeaponSystem = GetComponent<WeaponSystem>();
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

		public Transform GetTransform()
		{
			return transform;
		}
	}
}