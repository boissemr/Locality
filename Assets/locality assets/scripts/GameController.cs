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
		while(Mathf.Abs(sunlight.intensity - value) > .01f) {
			sunlight.intensity = Mathf.Lerp(sunlight.intensity, value, Time.deltaTime / duration);
			yield return new WaitForEndOfFrame();
		}
	}
}
