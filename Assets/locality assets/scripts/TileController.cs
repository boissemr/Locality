using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public Material		hoverMaterial;
	public GameObject[]	highlights,
						buildings;

	MeshRenderer		r;
	Material			defaultMaterial;

	void Start() {
		
		r = GetComponent<MeshRenderer>();
		defaultMaterial = r.material;

		foreach(GameObject o in highlights) {
			o.SetActive(false);
		}
	}

	void OnMouseOver() {
		r.material = hoverMaterial;
	}

	void OnMouseExit() {
		r.material = defaultMaterial;
	}

	void OnMouseDown() {

		foreach(GameObject o in highlights) {
			o.SetActive(!o.activeSelf);
		}

		/*
		GameObject temp = (GameObject)GameObject.Instantiate(buildings[(int)(Random.value * buildings.Length)], transform.position + new Vector3(-.5f, .05f, -.5f), Quaternion.identity);
		temp.transform.parent = transform;
		*/
	}
}
