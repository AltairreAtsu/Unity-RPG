using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehavior : MonoBehaviour, ISpecialAbility
	{
		private PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig config)
		{
			this.config = config;
		}

		public void Use(AbilityUseParams args)
		{
			print("Power Attack used!");
			args.target.TakeDamage(args.baseDamage + config.GetBonusDamage());
		}
	}
}

