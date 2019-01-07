using UnityEngine;

namespace RPG.Core
{
	public interface IDamagable
	{
		void TakeDamage(float damage);
		void Heal(float amount);
		Transform GetTransform();
	}
}

