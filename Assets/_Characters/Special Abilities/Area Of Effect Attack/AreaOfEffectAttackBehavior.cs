using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectAttackBehavior : AbilityBehavior<AreaOfEffectAttackConfig>
	{
		public override void Use(AbilityUseParams args)
		{
			DealRadialDamage(args);
			PlayEffect(args, stickToCaster: false);
		}

		private void DealRadialDamage(AbilityUseParams args)
		{
			Collider[] colliders = Physics.OverlapSphere(args.self.transform.position, config.GetRadius());
			foreach (Collider collider in colliders)
			{
				if (collider.gameObject == args.self) { continue; }

				var damageable = collider.GetComponent<Health>();

				if (damageable != null)
				{
					damageable.TakeDamage(args.baseDamage + config.GetBonusDamage());
				}
			}
		}
	}
}

