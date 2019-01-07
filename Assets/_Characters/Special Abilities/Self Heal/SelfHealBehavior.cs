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

		private static void HealCaster(AbilityUseParams args)
		{
			var health = args.self.GetComponent<IDamagable>();
			health.Heal(10f);
		}

		public void SetConfig(SelfHealConfig config)
		{
			this.config = config;
		}

		private void PlayEffect(AbilityUseParams args)
		{
			var vfx = Instantiate(config.GetCompoundParticleSystem(), args.self.transform);
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
	}
}

