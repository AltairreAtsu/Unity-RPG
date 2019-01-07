using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName = "RPG/Special Ability/Self Heal")]
	public class SelfHealConfig : AbilityConfig
	{
		[Header("Self Heal Specific:")]
		[SerializeField] private float healAmount;

		public override void AddComponent(GameObject gameobjectToAttachTo)
		{
			behavior = gameobjectToAttachTo.AddComponent<SelfHealBehavior>();
			var behave = (SelfHealBehavior)behavior;
			behave.SetConfig(this);
		}

		public float GetHealAmount()
		{
			return healAmount;
		}
	}
}

