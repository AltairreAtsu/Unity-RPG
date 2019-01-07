using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using RPG.Core;

namespace RPG.Characters
{
	public struct AbilityUseParams
	{
		public GameObject self;
		public IDamagable target;
		public float baseDamage;

		public AbilityUseParams(GameObject self, IDamagable target, float baseDamage)
		{
			this.self = self;
			this.target = target;
			this.baseDamage = baseDamage;
		}
	}

	public abstract class AbilityConfig : ScriptableObject
	{
		public float EnergyCost { get { return energyCost; } }

		[Header("Special Ability General")]
		[SerializeField] private KeyCode key;
		[SerializeField] private float energyCost = 10f;
		[SerializeField] private GameObject compoundParticleSystem;
		[SerializeField] private AudioClip soundClip;

		protected ISpecialAbility behavior;

		abstract public void AddComponent(GameObject gameobjectToAttachTo);

		public void Use(AbilityUseParams args)
		{
			behavior.Use(args);
		}

		public KeyCode GetKey()
		{
			return key;
		}

		public GameObject GetCompoundParticleSystem()
		{
			return compoundParticleSystem;
		}

		public AudioClip GetSoundClip()
		{
			return soundClip;
		}
	}

	public interface ISpecialAbility
	{
		void Use(AbilityUseParams args);
	}
}

