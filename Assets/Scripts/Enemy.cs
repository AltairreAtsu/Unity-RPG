using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour
{
	[SerializeField] private float maxHealthPoints = 10f;
	[SerializeField] private float currentHealth = 10f;

	private AICharacterControl aiCharacter;
	[SerializeField] private float attackRadius = 10f;
	[SerializeField] private Transform playerTransform;

	public float healthAsPercentage
	{
		get
		{
			return currentHealth / maxHealthPoints;
		}
	}

	private void Start()
	{
		aiCharacter = GetComponent<AICharacterControl>();
	}

	private void Update()
	{
		float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
		if(distanceToPlayer <= attackRadius)
		{
			aiCharacter.SetTarget(playerTransform);
		}
		else
		{
			aiCharacter.SetTarget(null);
		}	
	}
}
