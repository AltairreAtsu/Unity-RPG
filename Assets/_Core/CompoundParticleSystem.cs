using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
	public class CompoundParticleSystem : MonoBehaviour
	{
		[SerializeField] ParticleSystem[] particleSystems = null;
		[SerializeField] private float totalDuration = 0f;

		private AudioSource audioSource;
		private float longestParticleLifetime = 0f;

		public void InitAndPlay(bool selfDestruct)
		{
			Init();
			PlayAll();
			if (selfDestruct)
			{
				DestroyAfterPlaying();
			}
		}

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

			audioSource = GetComponent<AudioSource>();
			if(audioSource != null && audioSource.clip != null 
				&& audioSource.clip.length > totalDuration)
			{
				totalDuration = audioSource.clip.length;
			}
		}

		public void CopyRotationAndPositon(Transform transformToCopy)
		{
			transform.position = transformToCopy.position;
			transform.rotation = transformToCopy.rotation;
		}

		public void PlayAll()
		{
			foreach (ParticleSystem particleSystem in particleSystems)
			{
				particleSystem.Play();
			}
			if (audioSource != null)
			{
				audioSource.Play();
			}
		}

		public void DestroyAfterPlaying()
		{
			Destroy(gameObject, totalDuration);
		}
	}

}
