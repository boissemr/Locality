using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public Material		hoverMaterial;
	public GameObject[]	buildings,
						renderedBuildings;
	public GameObject	displayWhileSelected;
	public Building		myBuilding;
	public LayerMask	layerMaskTiles,
						layerMaskDefault;

	MeshRenderer		r;
	Material			defaultMaterial;
	bool				selected;
	RaycastHit[]		neighboringTiles;
	Building[]			neighboringBuildings;

	void Start() {

		// find and save default material
		r = GetComponent<MeshRenderer>();
		defaultMaterial = r.material;

		// disable highlights and canvas
		displayWhileSelected.SetActive(false);
	}

	void Update() {
		if(neighboringTiles != null) {
			for(int i = 0; i > neighboringTiles.Length; i++) {
				if(neighboringBuildings[i] != null)
					Debug.DrawLine(transform.position + Vector3.up, neighboringBuildings[i].transform.position);
			}
		}
	}

	// change material according to hover state
	void OnMouseOver() { r.material = hoverMaterial; }
	void OnMouseExit() { r.material = defaultMaterial; }

	void OnMouseDown() {

		// update buildings
		updateBuildings();
		
		// toggle selected status
		setSelected(!selected);
	}

	// find out which buildings can be built here
	public void updateBuildings() {

		// find neighboring tiles (exclude self)
		gameObject.layer = 8 << layerMaskDefault;
		neighboringTiles = Physics.SphereCastAll(transform.position, 1, Vector3.up, 1, layerMaskTiles);
		gameObject.layer = 8 << layerMaskTiles;

		//Debug.Log(neighboringTiles.Length);

		// find which buildings are in neighboring tiles
		neighboringBuildings = new Building[neighboringTiles.Length];
		for(int i = 0; i < neighboringTiles.Length; i++) {
			neighboringBuildings[i] = neighboringTiles[i].transform.gameObject.GetComponent<TileController>().myBuilding;
			if(neighboringBuildings[i] != null) {
				Debug.Log(neighboringBuildings[i].name);
			} else {
				Debug.Log("-");
			}
		}
	}

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
					instantiatePosition += new Vector3(1, -0.1f, -1);
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
