using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectAttackBehavior : AbilityBehavior
	{
		public override void Use(AbilityUseParams args)
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
			var aoeConfig = (AreaOfEffectAttackConfig)config;
			Collider[] colliders = Physics.OverlapSphere(args.self.transform.position, aoeConfig.GetRadius());
			foreach (Collider collider in colliders)
			{
				if (collider.gameObject == args.self) { continue; }

				var damageable = collider.GetComponent<IDamagable>();

				if (damageable != null)
				{
					damageable.TakeDamage(args.baseDamage + aoeConfig.GetBonusDamage());
				}
			}
		}
	}
}

