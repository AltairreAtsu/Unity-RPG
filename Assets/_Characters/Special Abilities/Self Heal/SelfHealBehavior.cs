﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public class SelfHealBehavior : AbilityBehavior<SelfHealConfig>
	{
		public override void Use(AbilityUseParams args)
		{
			HealCaster(args);
			PlayEffect(args, stickToCaster: true);
			PlayAbilityAnimation(args);
		}

		private void HealCaster(AbilityUseParams args)
		{
			var health = args.self.GetComponent<Health>();
			health.Heal(config.GetHealAmount());
		}

	}
}

