using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectAttackBehavior : MonoBehaviour, ISpecialAbility
	{
		private AreaOfEffectAttackConfig config = null;

		public void SetConfig(AreaOfEffectAttackConfig config)
		{
			this.config = config;
		}

		public void Use(AbilityUseParams args)
		{
			DealRadialDamage(args);
			PlayEffect(args);
		}

		private void PlayEffect(AbilityUseParams args)
		{
			var vfx = Instantiate(config.GetCompoundParticleSystem());
			vfx.transform.position = args.self.transform.position;
			var effectSystem = vfx.GetComponent<CompoundParticleSystem>();
			effectSystem.callbackListeners += OnEffectComplete;
			effectSystem.Init();
			effectSystem.PlayAll();
		}

		private void OnEffectComplete(GameObject system)
		{
			Destroy(system);
		}

		private void DealRadialDamage(AbilityUseParams args)
		{
			Collider[] colliders = Physics.OverlapSphere(args.self.transform.position, config.GetRadius());
			foreach (Collider collider in colliders)
			{
				if (collider.gameObject == args.self) { continue; }

				var damageable = collider.GetComponent<IDamagable>();

				if (damageable != null)
				{
					damageable.TakeDamage(args.baseDamage + config.GetBonusDamage());
				}
			}
		}
	}
}

