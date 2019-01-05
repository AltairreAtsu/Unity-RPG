using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

using RPG.CameraUI;
using RPG.Weapons;
using RPG.Core;

namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamagable
	{
		[SerializeField] private float maxHealthPoints = 10f;
		[SerializeField] private float currentHealth = 10f;
		[Space]
		[SerializeField] private float baseDamage = 3f;
		[SerializeField] private AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] private Weapon heldWeapon = null;
		[SerializeField] private SpecialAbilityConfig abilityOne = null;
		[Header("Death Variables")]
		[SerializeField] float deathDelay = 3f;
		[SerializeField] AudioClipArray deathSoundClips = null;
		[SerializeField] AudioClipArray hurtSoundClips = null;

		private Animator animator;
		private AudioSource audioSource;
		private Energy energy;

		private Coroutine deathCorotune;

		private bool isAlive = true;
		private float lastDamageTime = 0f;
		public float healthAsPercentage { get { return currentHealth / maxHealthPoints; } }

		private void Start()
		{
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
			energy = GetComponent<Energy>();

			PutWeaponInHand();
			SetupRuntimeAnimator();

			FindObjectOfType<CameraRaycaster>().onMouseOverEnemy += OnMouseOverEnemy;

			abilityOne.AddComponent(gameObject);
		}

		private void OnMouseOverEnemy(Enemy enemy)
		{
			if (Input.GetMouseButtonDown(1))
			{
				if (energy.IsEnergyAvailable(abilityOne.EnergyCost))
				{
					energy.ConsumeEnergy(abilityOne.EnergyCost);
					var abilityParams = new AbilityUseParams(gameObject, enemy, baseDamage);
					abilityOne.Use(abilityParams);
				}
			}
		}

		private void SetupRuntimeAnimator()
		{
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController["DEFUALT_ATTACK"] = heldWeapon.GetAnimation();
		}

		private void PutWeaponInHand()
		{
			Transform weaponSocket = null;
			switch (heldWeapon.GetHand()){
				case Hand.OffHand:
					weaponSocket = RequestOffHand();
					break;
				case Hand.DominantHand:
					weaponSocket = RequestDominantHand();
					break;
			}

			var weapon = Instantiate(heldWeapon.GetWeaponPrefab(), weaponSocket);
			weapon.transform.localPosition = heldWeapon.getWeaponGrip().transform.position;
			weapon.transform.localRotation = heldWeapon.getWeaponGrip().transform.rotation;

			animator.SetFloat("AttackAnimationSpeedMultiplier", heldWeapon.GetAttackSpeedMultiplier());
		}

		private HandIndicator[] GetHands()
		{
			var hands = GetComponentsInChildren<HandIndicator>();
			int numberOfHands = hands.Length;
			Assert.IsFalse(numberOfHands <= 0, "No HandIndicators Found on Player, please add the component to the hand transforms!");
			Assert.IsFalse(numberOfHands > 2, "Too many HandIndicators scripts detected! Please ensure there are only two present on the player object!");
			return hands;
		}

		private Transform RequestOffHand()
		{
			var hands = GetHands();
			foreach (HandIndicator hand in hands)
			{
				if (!hand.Dominant)
				{
					return hand.transform;
				}
			}
			return null;
		}

		private Transform RequestDominantHand()
		{
			var hands = GetHands();
			foreach (HandIndicator hand in hands)
			{
				if (hand.Dominant)
				{
					return hand.transform;
				}
			}
			return null;
		}

		public void TakeDamage(float damage)
		{
			currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealthPoints);
			if (currentHealth <= 0)
			{
				if (deathCorotune != null) { return; }
				deathCorotune = StartCoroutine(KillPlayer());
			}
			else
			{
				hurtSoundClips.PlayClip(audioSource);
			}
		}

		private IEnumerator KillPlayer()
		{
			isAlive = false;
			deathSoundClips.PlayClip(audioSource);
			animator.SetTrigger("Die");
			yield return new WaitForSecondsRealtime(deathDelay);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		public void TryAttack (Enemy enemy)
		{
			var target = enemy.GetComponent<IDamagable>();
			if (target == null) { return; }

			if (CanAttack(enemy.transform.position))
			{
				Attack(target, enemy.gameObject);
			}
		}

		private void Attack(IDamagable target, GameObject targetObject)
		{
			transform.LookAt(targetObject.transform);

			animator.SetTrigger("Attack");
			target.TakeDamage(baseDamage);
			lastDamageTime = Time.time;
			animatorOverrideController["DEFUALT_ATTACK"] = heldWeapon.GetAnimation();
		}

		private bool CanAttack(Vector3 position)
		{	
			var AttackCooldown = (Time.time - lastDamageTime < heldWeapon.GetAttackCooldown());

			return (!AttackCooldown) && (InRange(position));
		}

		public bool InRange(Vector3 position)
		{
			var distanceToPlayer = Vector3.Distance(transform.position, position);
			return distanceToPlayer <= heldWeapon.GetAttackRange();
		}

		public Transform GetTransform()
		{
			return transform;
		}

		public bool IsAlive()
		{
			return isAlive;
		}
	}
}