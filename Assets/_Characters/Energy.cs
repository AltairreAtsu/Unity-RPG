using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using RPG.CameraUI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour
	{
		[SerializeField] private RawImage EnergyBar = null;
		[SerializeField] private float maxEnergy = 100f;
		[SerializeField] private float energyPerAttack = 10;

		private float currentEnergy;
		
		private void Start()
		{
			currentEnergy = maxEnergy;
			FindObjectOfType<CameraRaycaster>().notifyRightClickObservers += OnRightClick;
		}

		private void Update()
		{
			float xValue = -(currentEnergy / maxEnergy / 2f) - 0.5f;
			EnergyBar.uvRect = new Rect(xValue, 0f, 0.5f, 1f);
		}

		private void OnRightClick()
		{
			currentEnergy -= energyPerAttack;
		}
	}

}
