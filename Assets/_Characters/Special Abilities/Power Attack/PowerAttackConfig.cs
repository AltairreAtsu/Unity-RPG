using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName ="RPG/Special Ability/Power Attack")]
	public class PowerAttackConfig : SpecialAbilityConfig
	{
		[Header("Power Attack Specific")]
		[SerializeField] float bonusDamage = 10f;

		public override void AddComponent(GameObject gameobjectToAttachTo)
		{
			behavior = gameobjectToAttachTo.AddComponent<PowerAttackBehavior>();
			var behave = (PowerAttackBehavior)behavior;
			behave.SetConfig(this);
		}

		public float GetBonusDamage()
		{
			return bonusDamage;
		}
	}
}

