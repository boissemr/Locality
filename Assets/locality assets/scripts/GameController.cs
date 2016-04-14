using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject	tilePrefab;
	public Light		sunlight;

	public void expand() {
		GameObject child = (GameObject)Instantiate(tilePrefab, transform.position, Quaternion.identity);
		child.transform.parent = transform;
	}

	public IEnumerator setSunlight(float value, float duration) {
		
		float timer = duration;

		while(timer > 0) {
			
			timer -= Time.deltaTime;
			sunlight.intensity = Mathf.Lerp(sunlight.intensity, value, duration);

			yield return new WaitForEndOfFrame();
		}
	}
}
