using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class SpecialAbilities : MonoBehaviour
	{
		[SerializeField] private AudioClipArray outOfEnergySound;
		[SerializeField] private Image energyBar = null;
		[SerializeField] private float maxEnergy = 100f;
		[SerializeField] private float energyRegenPerSecond = 5;
		[SerializeField] private AbilityConfig[] abilities = null;

		private AudioSource audioSource;
		private Player player;

		private float currentEnergy;

		private void Start()
		{
			audioSource = GetComponent<AudioSource>();
			player = GetComponent<Player>();

			currentEnergy = maxEnergy;
			if (energyBar != null) UpdateSliderGraphic();

			AttachSpecialAbilities();
		}

		private void Update()
		{
			if(energyBar != null) UpdateSliderGraphic();
			currentEnergy += energyRegenPerSecond * Time.deltaTime;
			currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
		}

		private void UpdateSliderGraphic()
		{
			energyBar.fillAmount = currentEnergy / maxEnergy;
		}

		private void ConsumeEnergy(float amount)
		{
			currentEnergy -= amount;
			currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
		}

		private void AttachSpecialAbilities()
		{
			foreach (AbilityConfig ability in abilities)
			{
				ability.AddComponent(gameObject);
			}
		}

		public void TryPerformPowerAttack(Health target)
		{
			if (!(currentEnergy >= abilities[0].EnergyCost) )
			{
				if (audioSource != null) outOfEnergySound.PlayClip(audioSource);
				return;
			}
			ConsumeEnergy(abilities[0].EnergyCost);
			var abilityParams = new AbilityUseParams(gameObject, target, player.WeaponSystem.BaseDamage);
			abilities[0].Use(abilityParams);
		}

		public void TryPerformSpecialAbility(int index)
		{
			var ability = abilities[index];
			if (!(currentEnergy >= ability.EnergyCost))
			{
				if(audioSource != null) outOfEnergySound.PlayClip(audioSource);
				return;
			}
			ConsumeEnergy(ability.EnergyCost);
			AbilityUseParams args = new AbilityUseParams(gameObject, null, player.WeaponSystem.BaseDamage);
			ability.Use(args);
		}
	}

}
