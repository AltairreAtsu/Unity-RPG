using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="RPG/Audio/Clip Array")]
public class AudioClipArray : ScriptableObject
{
	[SerializeField] private AudioClip[] clips = null;
	[SerializeField] private float minVolume = 0;
	[SerializeField] private float maxVolume = 0;
	[SerializeField] private float minPitch = 0;
	[SerializeField] private float maxPitch = 0;

	private AudioClip GetAudioClip()
	{
		return clips[Random.Range(0, clips.Length)];
	}

	private float GetRandomVolume()
	{
		return Random.Range(minVolume, maxVolume);
	}
	private float GetRandomPitch()
	{
		return Random.Range(minPitch, maxPitch);
	}

	public void PlayClip(AudioSource audioSource)
	{
		audioSource.volume = GetRandomVolume();
		audioSource.pitch = GetRandomPitch();
		audioSource.PlayOneShot(GetAudioClip());
		audioSource.pitch = 1f;
	}
}
