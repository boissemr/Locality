using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject	tilePrefab;

	public void expand() {
		GameObject child = (GameObject)Instantiate(tilePrefab, transform.position, Quaternion.identity);
		child.transform.parent = transform;
	}
}
