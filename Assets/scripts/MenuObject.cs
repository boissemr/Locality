using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuObject : MonoBehaviour {

	float		opacity;
	Text[]		textSubObjects;
	Image[] 	imageSubObjects;

	// start
	void Start() {
		textSubObjects = GetComponentsInChildren<Text>();
		imageSubObjects = GetComponentsInChildren<Image>();
	}

	// move to vector3
	public IEnumerator moveTo(Vector3 targetPosition, float duration) {
		while(Vector3.Distance(targetPosition, transform.position) > .01f) {
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / duration);
			yield return new WaitForEndOfFrame();
		}
	}

	// fade to opacity
	public void fadeTo(float opacity, float duration) {
		foreach(Text o in textSubObjects)	o.CrossFadeAlpha(opacity, duration, false);
		foreach(Image o in imageSubObjects)	o.CrossFadeAlpha(opacity, duration, false);
	}

	// toggle raycast target
	public void setRaycastTarget(bool state) {
		foreach(Text o in textSubObjects)	o.raycastTarget = false;
		foreach(Image o in imageSubObjects)	o.raycastTarget = false;
	}
}
