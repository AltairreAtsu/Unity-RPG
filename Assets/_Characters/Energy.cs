using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] private RawImage EnergyBar = null;
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
			float xValue = -(currentEnergy / maxEnergy / 2f) - 0.5f;
			EnergyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
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
