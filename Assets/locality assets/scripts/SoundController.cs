using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	public AudioClip[] tracks;
	public float fadeDuration;

	AudioSource source;

	void Start() {
		source = GetComponent<AudioSource>();
	}

	public void playTrack(int index) {
		source.Stop();
		source.clip = tracks[index];
		source.Play();
	}
}
