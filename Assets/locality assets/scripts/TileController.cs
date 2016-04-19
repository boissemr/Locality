using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public Material		hoverMaterial;
	public GameObject[]	buildings;
	public GameObject	displayWhileSelected;

	MeshRenderer		r;
	Material			defaultMaterial;
	bool				selected;

	void Start() {

		// find and save default material
		r = GetComponent<MeshRenderer>();
		defaultMaterial = r.material;

		// disable highlights and canvas
		displayWhileSelected.SetActive(false);
	}

	// change material according to hover state
	void OnMouseOver() { r.material = hoverMaterial; }
	void OnMouseExit() { r.material = defaultMaterial; }

	public void setSelected(bool state) {
		
		// change selected state
		selected = state;

		// toggle highlights and canvas
		displayWhileSelected.SetActive(selected);

		if(selected) {

			// de-select all other tiles
			foreach(TileController o in transform.parent.GetComponentsInChildren<TileController>()) {
				if(o != this) {
					o.setSelected(false);
				}
			}
		}
	}

	void OnMouseDown() {

		// toggle selected status
		setSelected(!selected);

		/*
		GameObject temp = (GameObject)GameObject.Instantiate(buildings[(int)(Random.value * buildings.Length)], transform.position + new Vector3(-.5f, .05f, -.5f), Quaternion.identity);
		temp.transform.parent = transform;
		*/
	}
}
