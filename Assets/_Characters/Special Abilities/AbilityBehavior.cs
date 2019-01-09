using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public abstract class AbilityBehavior<T> : MonoBehaviour, ISpecialAbility where T: AbilityConfig
	{
		private const string SPECIAIL_ABILITY_ANIM = "SPECIAL_ABILITY";
		private const string SPECIAL_ABILITY_TRIGGER = "Special Ability";

		protected T config;
		
		public void SetConfig(AbilityConfig config)
		{
			this.config = (T)config;
		}

		abstract public void Use(AbilityUseParams args);

		protected void PlayEffect(AbilityUseParams args, bool stickToCaster)
		{
			CompoundParticleSystem vfxSystem = null;
			if (stickToCaster)
			{
				vfxSystem = Instantiate(config.GetCompoundParticleSystem(), args.self.transform).GetComponent<CompoundParticleSystem>();
			}
			else
			{
				vfxSystem = Instantiate(config.GetCompoundParticleSystem()).GetComponent<CompoundParticleSystem>();
				vfxSystem.CopyRotationAndPositon(args.self.transform);
			}

			vfxSystem.InitAndPlay(selfDestruct: true);
		}

		protected void PlayAbilityAnimation(AbilityUseParams args)
		{
			var animation = config.GetAbilityAnimation();
			animation = RemoveAnimationEvents(animation);
			var overrideController = args.self.GetComponent<Character>().OverrideAnimator;
			var animator = args.self.GetComponent<Animator>();

			overrideController[SPECIAIL_ABILITY_ANIM] = animation;
			animator.SetTrigger(SPECIAL_ABILITY_TRIGGER);
		}

		private AnimationClip RemoveAnimationEvents(AnimationClip animationClip)
		{
			animationClip.events = new AnimationEvent[0];
			return animationClip;
		}
	}

}
