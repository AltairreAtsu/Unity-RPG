using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public class SelfHealBehavior : MonoBehaviour, ISpecialAbility
	{
		private SelfHealConfig config;

		public void Use(AbilityUseParams args)
		{
			HealCaster(args);
			PlayEffect(args);
		}

		private void HealCaster(AbilityUseParams args)
		{
			var health = args.self.GetComponent<IDamagable>();
			health.Heal(config.GetHealAmount());
		}

		public void SetConfig(SelfHealConfig config)
		{
			this.config = config;
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

