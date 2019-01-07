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

	public abstract class SpecialAbilityConfig : ScriptableObject
	{
		public float EnergyCost { get { return energyCost; } }

		[Header("Special Ability General")]
		[SerializeField] private float energyCost = 10f;
		[SerializeField] private KeyCode key;
		[SerializeField] private GameObject CompoundParticleSystem;

		protected ISpecialAbility behavior;

		abstract public void AddComponent(GameObject gameobjectToAttachTo);

		public void Use(AbilityUseParams args)
		{
			behavior.Use(args);
		}

		public GameObject GetCompoundParticleSystem()
		{
			return CompoundParticleSystem;
		}

		public KeyCode GetKey()
		{
			return key;
		}
	}

	public interface ISpecialAbility
	{
		void Use(AbilityUseParams args);
	}
}

