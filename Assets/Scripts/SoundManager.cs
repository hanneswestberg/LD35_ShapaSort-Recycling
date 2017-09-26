using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour {

	[SerializeField] private AudioClip[] effectSounds;
	[SerializeField] private AudioClip[] garbageEffects;
	[SerializeField] private AudioClip uiClick;
	[SerializeField] private AudioClip[] doorSound;

	[SerializeField] private AudioSource effectAudioSource;
	[SerializeField] private AudioSource backgroundAudioSource;
	[SerializeField] private AudioSource doorsAudioSource;
	[SerializeField] private AudioSource truckAudioSource;


	public void PlayEffect(int clip){
		effectAudioSource.PlayOneShot(effectSounds[clip]);
	}

	public void PlayGarbageEffect(){
		int rand = Random.Range(0,garbageEffects.Length);
		effectAudioSource.PlayOneShot(garbageEffects[rand]);
	}

	public void PlayDoorEffect(){
		int rand = Random.Range(0,doorSound.Length);
		doorsAudioSource.PlayOneShot(doorSound[rand]);
	}

	public void PlayUIClick(){
		effectAudioSource.PlayOneShot(uiClick);
	}

	public void PlayTruckSound(int clip){
		truckAudioSource.PlayOneShot(effectSounds[clip]);
	}
}
