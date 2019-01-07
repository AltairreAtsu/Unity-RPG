using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public abstract class AbilityBehavior : MonoBehaviour
	{
		protected AbilityConfig config;
		
		public void SetConfig(AbilityConfig config)
		{
			this.config = config;
		}

		abstract public void Use(AbilityUseParams args);

		private void PlayEffect(AbilityUseParams args, bool stickToCaster)
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

	}

}
