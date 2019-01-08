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

		Rigidbody rigidBody;
		Animator animator;
		Player player;
		NavMeshAgent agent;
		Transform target;

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

		public void Move(Vector3 move)
		{

			// convert the world relative moveInput vector into a local-relative
			// turn amount and forward amount required to head in the desired
			// direction.
			if (move.magnitude > 1f) move.Normalize();
			move = transform.InverseTransformDirection(move);
			move = Vector3.ProjectOnPlane(move, Vector3.zero);
			var turnAmount = Mathf.Atan2(move.x, move.z);
			var forwardAmount = move.z;

			ApplyExtraTurnRotation(turnAmount, forwardAmount);

			// send input and other state parameters to the animator
			UpdateAnimator(move, turnAmount, forwardAmount);
		}

		void UpdateAnimator(Vector3 move, float turnAmount, float forwardAmount)
		{
			// update the animator parameters
			animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
			animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
		}

		void ApplyExtraTurnRotation(float turnAmount, float forwardAmount)
		{
			// help the character turn faster (this is in addition to root rotation in the animation)
			float turnSpeed = Mathf.Lerp(stationaryTurnSpeed, movingTurnSpeed, forwardAmount);
			transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
		}

		public void OnAnimatorMove()
		{
			// we implement this function to override the default root motion.
			// this allows us to modify the positional speed before it's applied.
			if (Time.deltaTime > 0)
			{
				Vector3 v = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

				// we preserve the existing y part of the current velocity.
				v.y = rigidBody.velocity.y;
				rigidBody.velocity = v;
			}
		}
	}
}