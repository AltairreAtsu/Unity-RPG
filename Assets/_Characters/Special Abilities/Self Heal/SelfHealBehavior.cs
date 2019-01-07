using System.Collections;
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
			PlayEffect(args);
		}

		private void HealCaster(AbilityUseParams args)
		{
			var health = args.self.GetComponent<IDamagable>();
			health.Heal(config.GetHealAmount());
		}

		private void PlayEffect(AbilityUseParams args)
		{
			var vfxSystem = Instantiate(config.GetCompoundParticleSystem(), args.self.transform)
				.GetComponent<CompoundParticleSystem>();
			vfxSystem.transform.position = args.self.transform.position;
			vfxSystem.InitAndPlay(selfDestruct: true);
		}
	}
}

