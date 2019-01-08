using System;
using UnityEngine;


using UnityEngine.AI;

namespace RPG.Characters
{
	public class CharacterMovement : MonoBehaviour
	{

		[SerializeField] float movingTurnSpeed = 360;
		[SerializeField] float stationaryTurnSpeed = 180;
		[SerializeField] float moveSpeedMultiplier = 1f;
		[SerializeField] float animationSpeedMultiplier = 1f;

		Rigidbody rigidBody;
		Animator animator;
		Player player;
		NavMeshAgent agent;
		Transform target;

		float forwardAmount;
		float turnAmount;

		private void Start()
		{
			GetDependencies();

			rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
		}

		private void GetDependencies()
		{
			animator = GetComponent<Animator>();
			rigidBody = GetComponent<Rigidbody>();
			player = GetComponent<Player>();
			agent = GetComponent<NavMeshAgent>();
			agent.updatePosition = true;
			agent.updateRotation = false;
		}

		private void Update()
		{
			if(target == null) { return; }
			if (agent.remainingDistance > agent.stoppingDistance)
			{
				Move(agent.desiredVelocity);
			}
			else
			{
				Move(Vector3.zero);
			}
			agent.SetDestination(target.position);
		}

		public void SetTarget(Transform target)
		{
			this.target = target;
		}

		public void Move(Vector3 movement)
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