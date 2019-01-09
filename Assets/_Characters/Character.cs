using System;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Characters
{
	[SelectionBase]
	public class Character : MonoBehaviour
	{
		[Header("Animator:")]
		[SerializeField] RuntimeAnimatorController controller;
		[SerializeField] AnimatorOverrideController overrideController;
		[SerializeField] Avatar avatar;

		[Header("Audiosource:")]
		[SerializeField] bool mute = false;
		[SerializeField] [Range(0, 256)] int priority = 128;
		[SerializeField][Range(0, 1)] float volume = 1f;
		[SerializeField] [Range(-3f, 3f)] float pitch = 1f;
		[SerializeField] [Range(0, 1)] float spatialBlend = 0f;

		[Header("Capsule Collider:")]
		[SerializeField] Vector3 center = new Vector3(0f, 0.8f, 0f);
		[SerializeField] float radius = 0.3f;
		[SerializeField] float height = 1.6f;

		[Header("Movement And Animation:")]
		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animationSpeedMultiplier = 1f;

		[Header("Nav Mesh Agent:")]
		[SerializeField] float speed = 3.5f;
		[SerializeField] float angularSpeed = 120f;
		[SerializeField] float acceleration = 8f;
		[SerializeField] float stoppingDistance = 1.3f;
		[SerializeField] float agentRadius = 0.5f;
		[SerializeField] float agentHeight = 2f;

		Animator animator;
		Health health;
		NavMeshAgent navAgent;
		Rigidbody rigidBody;
		Transform targetTransform;

		Vector3 targetPosition;

		float forwardAmount;
		float turnAmount;

		public AnimatorOverrideController OverrideAnimator { get { return overrideController; } }

		private void Awake()
		{
			AddRequiredComponents();
		}

		private void AddRequiredComponents()
		{
			animator = gameObject.AddComponent<Animator>();
			if (overrideController)
			{
				animator.runtimeAnimatorController = overrideController;
			}
			else
			{
				animator.runtimeAnimatorController = controller;
			}
			animator.avatar = avatar;

			var audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.mute = mute;
			audioSource.priority = priority;
			audioSource.volume = volume;
			audioSource.pitch = pitch;
			audioSource.spatialBlend = spatialBlend;

			var collider = gameObject.AddComponent<CapsuleCollider>();
			collider.center = center;
			collider.radius = radius;
			collider.height = height;

			rigidBody = gameObject.AddComponent<Rigidbody>();
			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

			navAgent = gameObject.AddComponent<NavMeshAgent>();
			navAgent.speed = speed;
			navAgent.angularSpeed = angularSpeed;
			navAgent.acceleration = acceleration;
			navAgent.stoppingDistance = stoppingDistance;
			navAgent.radius = agentRadius;
			navAgent.height = agentHeight;
			navAgent.autoBraking = false;
			navAgent.updatePosition = true;
			navAgent.updateRotation = false;

			health = GetComponent<Health>();
		}

		private void Update()
		{
			if(!health.Alive) { return; }
			if (navAgent.remainingDistance > navAgent.stoppingDistance)
			{
				Move(navAgent.desiredVelocity);
			}
			else
			{
				Move(Vector3.zero);
			}

			if (targetTransform)
			{
				navAgent.SetDestination(targetTransform.position);
			}
			else if (targetPosition != Vector3.zero)
			{
				navAgent.SetDestination(targetPosition);
			}
			
		}

		public void SetTarget(Transform targetTransform)
		{
			this.targetTransform = targetTransform;
			targetPosition = Vector3.zero;
		}
		public void SetTarget(Vector3 targetPosition)
		{
			this.targetPosition = targetPosition;
			targetTransform = null;
		}

		private void Move(Vector3 movement)
		{
			SetForwardAndTurn(movement);
			ApplyExtraTurnRotation();
			UpdateAnimator(movement);
		}

		private void SetForwardAndTurn(Vector3 movement)
		{
			if (movement.magnitude > 1f)
			{
				movement.Normalize();
			}
			var localMovement = transform.InverseTransformDirection(movement);
			turnAmount = Mathf.Atan2(localMovement.x, localMovement.z);
			forwardAmount = localMovement.z;
		}

		void UpdateAnimator(Vector3 move)
		{
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);

			if (move.magnitude > 0)
			{
				animator.speed = animationSpeedMultiplier;
			}

		}

		void ApplyExtraTurnRotation()
		{
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

		public void OnAnimatorMove()
		{
			if (Time.deltaTime > 0)
			{
				Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;
				v.y = rigidBody.velocity.y;
				rigidBody.velocity = v;
			}
		}
	}
}