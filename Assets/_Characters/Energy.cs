using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] private Image energyBar = null;
		[SerializeField] private float maxEnergy = 100f;
		[SerializeField] private float energyRegenPerSecond = 5;

		private float currentEnergy;

		private void Start()
		{
			currentEnergy = maxEnergy;
		}

		private void Update()
		{
			UpdateSliderGraphic();
			currentEnergy += energyRegenPerSecond * Time.deltaTime;
			currentEnergy = Mathf.Clamp(currentEnergy, 0f, maxEnergy);
		}

		public void UpdateSliderGraphic()
		{
			energyBar.fillAmount = currentEnergy / maxEnergy;
		}

		public bool IsEnergyAvailable (float requiredAmmount)
		{
			return currentEnergy >= requiredAmmount;
		}

		public void ConsumeEnergy(float amount)
		{
			currentEnergy -= amount;
			currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
		}
	}

}
