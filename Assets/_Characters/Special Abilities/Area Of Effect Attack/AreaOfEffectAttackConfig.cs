using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName ="RPG/Special Ability/Area of Effect")]
	public class AreaOfEffectAttackConfig : AbilityConfig
	{
		[Header("Area of Effect Specific:")]
		[SerializeField] private float radius = 5f;
		[SerializeField] private float bonusDamage = 5f;

		public override void AddComponent(GameObject gameobjectToAttachTo)
		{
			behavior = gameobjectToAttachTo.AddComponent<AreaOfEffectAttackBehavior>();
			var behave = (AreaOfEffectAttackBehavior)behavior;
			behave.SetConfig(this);
		}

		public float GetRadius()
		{
			return radius;
		}

		public float GetBonusDamage()
		{
			return bonusDamage;
		}
	}
}

