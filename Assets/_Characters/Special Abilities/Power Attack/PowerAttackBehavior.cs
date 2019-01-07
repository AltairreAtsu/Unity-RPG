using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehavior : AbilityBehavior<PowerAttackConfig>
	{
		public override void Use(AbilityUseParams args)
		{
			args.target.TakeDamage(args.baseDamage + config.GetBonusDamage());
		}
	}
}

