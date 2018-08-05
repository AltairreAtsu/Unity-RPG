using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawlerCamera : MonoBehaviour {
	[SerializeField] private Transform target = null;

	[SerializeField] private float dampening = 1f;

	[SerializeField] private bool alwaysLookAtPlayer = false;
	[SerializeField] private bool useDamping = true;

	private Vector3 offset = Vector3.zero;

	private void Start()
	{
		if (target == null)
		{
			target = GameObject.FindWithTag("Player").transform;
			if (!target)
			{
				Debug.LogError("Camera Target is set to null and could not find object tagged as player!");
			}
		}

		offset = transform.position - target.position;
	}

	private void LateUpdate ()
	{
		Vector3 desiredPosition = target.transform.position + offset;

		if (useDamping)
		{
			transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * dampening);
		}
		else
		{
			transform.position = desiredPosition;
		}

		if (alwaysLookAtPlayer)
		{
			transform.LookAt(target);
		}
	}
}
