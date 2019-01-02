using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	[SerializeField] private float projectileSpeed = 10f;
	private float damageDealt = 3;

	private GameObject shooter;
	private Rigidbody rigidBody;

	private void OnTriggerEnter(Collider other)
	{
		IDamagable isDamagable = other.GetComponent<IDamagable>();
		if (isDamagable != null)
		{
			if(other.gameObject.layer != shooter.layer)
			{
				isDamagable.TakeDamage(damageDealt);
			}
		}
		// TODO replace with object poolins
		Destroy(gameObject);
	}

	public void Launch(Vector3 target)
	{
		var dirrectionalVector = Vector3.Normalize(target - transform.position);
		GetComponent<Rigidbody>().velocity = dirrectionalVector * projectileSpeed;
	}

	public void SetShooter(GameObject shooter)
	{
		this.shooter = shooter;
	}

	public void SetDamage(float damageDealt)
	{
		this.damageDealt = damageDealt;
	}

	public void SetSpeed(float projectileSpeed)
	{
		this.projectileSpeed = projectileSpeed;
	}
}
