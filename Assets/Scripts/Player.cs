using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable {
	[SerializeField] private float maxHealthPoints = 10f;
	[SerializeField] private float currentHealth = 10f;
	[Space]
	[SerializeField] private int enemyLayer = 9;
	[SerializeField] private float damagePerShot = 3f;
	[SerializeField] private float damageDelay = 0.5f;
	[SerializeField] private float attackRange = 0.5f;
	private float lastDamageTime = 0f;

	public float healthAsPercentage { get { return currentHealth / maxHealthPoints; } }

	private void Start()
	{
		Camera.main.GetComponent<CameraRaycaster>().notifyMouseClickObservers += OnMouseClick;
	}

	public void TakeDamage(float damage)
	{
		currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealthPoints);
	}

	private void OnMouseClick(RaycastHit hit, int layerHit)
	{
		if (layerHit == enemyLayer)
		{
			var damable = hit.collider.GetComponent<IDamagable>();
			var distanceToPlayer = Vector3.Distance(transform.position, hit.collider.transform.position);

			if ( (damable != null) && (Time.time - lastDamageTime > damageDelay) && (distanceToPlayer <= attackRange ) )
			{
				damable.TakeDamage(damagePerShot);
				lastDamageTime = Time.time;
				//print(string.Format("Player dealing {0} damage!", damagePerShot.ToString()));
			}
		}
	}
}
