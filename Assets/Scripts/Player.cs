using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] private float maxHealthPoints = 10f;
	[SerializeField] private float currentHealth = 10f;

	public float healthAsPercentage
	{
		get
		{
			return currentHealth / maxHealthPoints;
		}
	}
}
