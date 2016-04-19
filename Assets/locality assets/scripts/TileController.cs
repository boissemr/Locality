using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public Material		hoverMaterial;
	public GameObject[]	buildings,
						renderedBuildings;
	public GameObject	displayWhileSelected;
	public Building		myBuilding;

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

	// toggle selected status
	void OnMouseDown() { setSelected(!selected); }

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

			if(myBuilding == null) {

				// show buildings to select from
				renderedBuildings = new GameObject[buildings.Length];
				Vector3 instantiatePosition = new Vector3(-buildings.Length / 2 + .5f, 0, buildings.Length / 2 - .5f);
				for(int i = 0; i < buildings.Length; i++) {
					renderedBuildings[i] = (GameObject)Instantiate(buildings[i], transform.position + instantiatePosition, Quaternion.identity);
					renderedBuildings[i].transform.parent = transform;
					instantiatePosition += new Vector3(1, 0, -1);
				}
			}
		} else {

			// hide buildings to select from
			foreach(GameObject o in renderedBuildings) {
				GameObject.Destroy(o);
			}
		}
	}

	public void setBuilding(Building o) {

		// set my building
		myBuilding = o;

		// remove this building from renderedBuildings so that it does not get deleted
		for(int i = 0; i < buildings.Length; i++) {
			if(renderedBuildings[i].name.CompareTo(myBuilding.name) == 0) {
				renderedBuildings[i] = null;
			}
		}

		// deselect this tile
		setSelected(false);
	}
}
