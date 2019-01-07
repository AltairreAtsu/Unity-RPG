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
			var vfxSystem = Instantiate(config.GetCompoundParticleSystem()).GetComponent<CompoundParticleSystem>();
			vfxSystem.transform.position = args.self.transform.position;
			vfxSystem.InitAndPlay(selfDestruct: true);
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

