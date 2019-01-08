using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class Health : MonoBehaviour
	{
		[SerializeField] private float maxHealthPoints = 100f;
		[SerializeField] private float deathDestroyDelay = 3f;
		[SerializeField] private AudioClipArray hurtSounds;
		[SerializeField] private AudioClipArray deathSounds;

	public delegate void OnDeath(float delay);
		public event OnDeath onDeathListeners;

		private const string DEATH_TRIGGER = "Death";

		private Animator animator;
		private AudioSource audioSource;
		private float currentHealth = 100f;
		private bool isAlive = true;

		public float HealthAsPercentage { get { return currentHealth / maxHealthPoints; } }
		public bool Alive { get { return isAlive; } }

		private void Start()
		{
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();

			currentHealth = maxHealthPoints;
		}

		public void TakeDamage(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealthPoints);
			if (currentHealth <= 0)
			{
				KillEntity();
			}
			else
			{
				if(hurtSounds == null) { return; }
				hurtSounds.PlayClip(audioSource);
			}
		}

		public void Heal(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealthPoints);
		}

		private void KillEntity()
		{
			isAlive = false;
			if(deathSounds != null) deathSounds.PlayClip(audioSource);
			var locomotion = GetComponent<CharacterMovement>();
			if (locomotion && locomotion.isActiveAndEnabled)
			{
				locomotion.enabled = false;
			}

			animator.SetTrigger(DEATH_TRIGGER);
			if (onDeathListeners != null)
			{
				onDeathListeners(deathDestroyDelay);
			}
			Destroy(gameObject, deathDestroyDelay);
		}
	}

	public interface IDisplayHealth
	{
		void UpdateDisplay(float healthPercent);
	}
}