using UnityEngine;

namespace RPG.Core
{
	public interface IDamagable
	{
		void TakeDamage(float damage);
		Transform GetTransform();
	}
}

