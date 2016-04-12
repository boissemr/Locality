using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {

	public float rotationPerSecond;

	void Update() {
		transform.Rotate(0, rotationPerSecond * Time.deltaTime, 0);
	}
}
