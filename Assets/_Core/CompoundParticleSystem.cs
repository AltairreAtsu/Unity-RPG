using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
	public class CompoundParticleSystem : MonoBehaviour
	{
		[SerializeField] ParticleSystem[] particleSystems = null;
		[SerializeField] private float totalDuration = 0f;

		public delegate void OnFinishedPlaying(GameObject system);
		public event OnFinishedPlaying callbackListeners;

		private Coroutine callbackCoroutine;
		private float longestParticleLifetime = 0f;

		public void Init()
		{
			if (particleSystems == null || particleSystems.Length == 0)
			{
				particleSystems = GetComponentsInChildren<ParticleSystem>();
			}

			if (totalDuration == 0f)
			{
				foreach (ParticleSystem particleSystem in particleSystems)
				{
					if (particleSystem.main.duration > totalDuration)
					{
						totalDuration = particleSystem.main.duration;
					}
					if(particleSystem.main.startLifetime.constantMax > longestParticleLifetime)
					{
						longestParticleLifetime = particleSystem.main.startLifetime.constantMax;
					}
				}
				totalDuration += longestParticleLifetime;
			}
		}

		public void PlayAll()
		{
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				if(callbackCoroutine == null)
				{
					callbackCoroutine = StartCoroutine(CallbackMethod());
				}
				particleSystem.Play();
			}
		}

		private IEnumerator CallbackMethod()
		{
			yield return new WaitForSeconds(totalDuration);
			callbackListeners(gameObject);
			callbackCoroutine = null;
		}
	}

}
