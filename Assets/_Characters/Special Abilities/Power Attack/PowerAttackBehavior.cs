using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehavior : AbilityBehavior
	{
		public override void Use(AbilityUseParams args)
		{
			var powerConfig = (PowerAttackConfig)config;
			args.target.TakeDamage(args.baseDamage + powerConfig.GetBonusDamage());
		}
	}
}

