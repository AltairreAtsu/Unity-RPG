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
		[SerializeField] private AbilityConfig[] abilities = null;
		[Header("Pickup Variables")]
		[SerializeField] private float pickupRange = 3f;
		[Header("Critical Hit Variables")]
		[SerializeField] [Range(0f, 1.0f)] float criticalHitChance = 0.1f;
		[SerializeField] float criticalHitMultiplier = 1.5f;
		[SerializeField] GameObject criticalHitVFX = null;
		[Header("Death Variables")]
		[SerializeField] float deathDelay = 3f;
		[SerializeField] AudioClipArray deathSoundClips = null;
		[SerializeField] AudioClipArray hurtSoundClips = null;

		private Animator animator;
		private AudioSource audioSource;
		private Energy energy;
		private GameObject weaponObject;

		private Coroutine deathCorotune;

		private bool isAlive = true;
		private float lastDamageTime = 0f;
		public float healthAsPercentage { get { return currentHealth / maxHealthPoints; } }

		private void Start()
		{
			animator = GetComponent<Animator>();
			audioSource = GetComponent<AudioSource>();
			energy = GetComponent<Energy>();

			PutWeaponInHand(heldWeapon);
			animator.runtimeAnimatorController = animatorOverrideController;

			var cameraRaycaster = FindObjectOfType<CameraRaycaster>();
			cameraRaycaster.onMouseOverEnemy += OnMouseOverEnemy;
			cameraRaycaster.onMouseOverPickup += OnMouseOverPickup;

			AttachSpecialAbilities();
		}

		private void Update()
		{
			if (!IsAlive()) { return; }
			ScanForAbilityInput();
		}

		private void ScanForAbilityInput()
		{
			foreach (AbilityConfig ability in abilities)
			{
				if(ability.GetKey() == KeyCode.None) { continue; }
				if (Input.GetKeyDown(ability.GetKey()))
				{
					if(!energy.IsEnergyAvailable(ability.EnergyCost)) { return; }
					energy.ConsumeEnergy(ability.EnergyCost);
					AbilityUseParams args = new AbilityUseParams(gameObject, null, baseDamage);
					ability.Use(args);
				}
			}
		}

		private void AttachSpecialAbilities()
		{
			foreach (AbilityConfig ability in abilities)
			{
				ability.AddComponent(gameObject);
			}
		}

		private void OnMouseOverEnemy(Enemy enemy)
		{
			if (!IsAlive()) { return; }
			if (Input.GetMouseButtonDown(1))
			{
				TryPerformPowerAttack(enemy);
			}
		}

		private void OnMouseOverPickup(WeaponPickupPoint pickup)
		{
			if( Vector3.Distance(transform.position, pickup.transform.position) <= pickupRange 
				&& Input.GetMouseButtonDown(0))
			{
				// Move towards weapon if not close enough?
				// Play Sound
				// Trigger Animation
				PutWeaponInHand(pickup.GetWeapon());
			}
		}

		private void TryPerformPowerAttack(Enemy enemy)
		{
			if (energy.IsEnergyAvailable(abilities[0].EnergyCost))
			{
				energy.ConsumeEnergy(abilities[0].EnergyCost);
				var abilityParams = new AbilityUseParams(gameObject, enemy, baseDamage);
				abilities[0].Use(abilityParams);
			}
		}

		private void PutWeaponInHand(Weapon weapon)
		{
			if(weaponObject != null)
			{
				Destroy(weaponObject);
			}
			Transform weaponSocket = null;
			switch (weapon.GetHand()){
				case Hand.OffHand:
					weaponSocket = RequestOffHand();
					break;
				case Hand.DominantHand:
					weaponSocket = RequestDominantHand();
					break;
			}

			weaponObject = Instantiate(weapon.GetWeaponPrefab(), weaponSocket);
			weaponObject.transform.localPosition = weapon.getWeaponGrip().transform.position;
			weaponObject.transform.localRotation = weapon.getWeaponGrip().transform.rotation;

			animator.SetFloat("AttackAnimationSpeedMultiplier", weapon.GetAttackSpeedMultiplier());
			heldWeapon = weapon;
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

		public void Heal(float amount)
		{
			currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealthPoints);
		}

		private IEnumerator KillPlayer()
		{
			isAlive = false;
			DisablePlayerMovement();

			deathSoundClips.PlayClip(audioSource);
			animator.SetTrigger("Die");
			yield return new WaitForSecondsRealtime(deathDelay);
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void DisablePlayerMovement()
		{
			var playerMovement = GetComponent<PlayerMovement>();
			playerMovement.UnSubscribeFromEvents();
			playerMovement.enabled = false;
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
			target.TakeDamage(CalculateDamage());
			lastDamageTime = Time.time;
			animatorOverrideController["DEFUALT_ATTACK"] = heldWeapon.GetAnimation();
		}


		private float CalculateDamage()
		{
			if (Random.value <= criticalHitChance)
			{
				PerformCriticalHit();
				return (baseDamage + heldWeapon.GetWeaponDamage()) * 3; 
			}
			else
			{
				return baseDamage + heldWeapon.GetWeaponDamage();
			}
		}

		private void PerformCriticalHit()
		{
			var vfxSystem = Instantiate(criticalHitVFX, transform).GetComponent<CompoundParticleSystem>();
			vfxSystem.InitAndPlay(selfDestruct: true);
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