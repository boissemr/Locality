using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

	public AudioClip[] tracks;
	public float fadeSpeed;

	AudioSource source;

	void Start() {
		source = GetComponent<AudioSource>();
	}

	public void playTrack(int index) {
		StartCoroutine("crossFade", index);
	}

	public IEnumerator crossFade(int index) {

		// save volume
		float storedVolume = source.volume;

		// fade out
		while(source.volume > 0) {
			source.volume -= Time.deltaTime * fadeSpeed;
			yield return new WaitForEndOfFrame();
		}

		// switch tracks
		source.Stop();
		source.clip = tracks[index];
		source.Play();

		// fade in
		while(source.volume < storedVolume) {
			source.volume += Time.deltaTime * fadeSpeed;
			yield return new WaitForEndOfFrame();
		}
		source.volume = storedVolume;
	}
}
