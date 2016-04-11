using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public Material		hoverMaterial;

	MeshRenderer		r;
	Material			defaultMaterial;

	void Start() {
		r = GetComponent<MeshRenderer>();
		defaultMaterial = r.material;
	}

	void OnMouseOver() {
		r.material = hoverMaterial;
	}

	void OnMouseExit() {
		r.material = defaultMaterial;
	}
}
