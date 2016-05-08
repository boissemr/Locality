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
	bool[]				canBeBuilt;
	int					numCanBeBuilt;
	GameController		gameController;

	void Start() {

		// find gameController
		gameController = GameObject.Find("gameController").GetComponent<GameController>();

		// find and save default material
		r = GetComponent<MeshRenderer>();
		defaultMaterial = r.material;

		// disable highlights and canvas
		displayWhileSelected.SetActive(false);

		// set up canBeBuilt
		canBeBuilt = new bool[buildings.Length];
		numCanBeBuilt = 0;
		for(int i = 0; i < buildings.Length; i++) {
			canBeBuilt[i] = false;
		}
	}

	// change material according to hover state
	void OnMouseOver() { r.material = hoverMaterial; }
	void OnMouseExit() { r.material = defaultMaterial; }

	void OnMouseDown() {
		
		// toggle selected status
		setSelected(!selected);
	}

	// find out which buildings can be built here
	public void updateBuildings() {

		// find neighboring tiles (3x3 grid, exclude self)
		gameObject.layer = 8 << layerMaskDefault;
		neighboringTiles = Physics.SphereCastAll(transform.position, 1, Vector3.up, 1, layerMaskTiles);
		gameObject.layer = 8 << layerMaskTiles;

		// find which buildings are in neighboring tiles
		neighboringBuildings = new Building[neighboringTiles.Length];
		for(int i = 0; i < neighboringTiles.Length; i++) {
			neighboringBuildings[i] = neighboringTiles[i].transform.gameObject.GetComponent<TileController>().myBuilding;
		}

		// find out which buildings can be built
		numCanBeBuilt = (gameController.localityCreated ? -1 : 0);
		for(int i = 0; i < buildings.Length; i++) {
			
			bool thisBuildingCanBeBuilt = true;

			// for each required neighbor
			foreach(Building requirement in buildings[i].GetComponent<Building>().requiredNeighbors) {

				// check if any of the neighboring buildings meets the requirement
				bool aNeighborMeetsTheRequirement = false;
				foreach(Building neighbor in neighboringBuildings) {
					if(neighbor != null) {
						if(neighbor.GetComponent<Building>().name.Substring(0, neighbor.GetComponent<Building>().name.Length - 7) == requirement.name) {
							aNeighborMeetsTheRequirement = true;
						}
					}
				}

				// if none did, this building can't be built
				if(!aNeighborMeetsTheRequirement) {
					thisBuildingCanBeBuilt = false;
				}
			}

			// can it be built?
			//Debug.Log(buildings[i].name + "\tcan" + (thisBuildingCanBeBuilt ? "" : " NOT" ) + " be built here.");
			if(thisBuildingCanBeBuilt) {
				canBeBuilt[i] = true;
				numCanBeBuilt += 1;
			}
			else {
				canBeBuilt[i] = false;
			}
		}
	}

	public void setSelected(bool state) {
		
		// change selected state
		selected = state;

		// toggle highlights and canvas
		displayWhileSelected.SetActive(selected);

		// select
		if(selected) {

			// update buildings
			updateBuildings();

			// de-select all other tiles
			foreach(TileController o in transform.parent.GetComponentsInChildren<TileController>()) {
				if(o != this) {
					o.setSelected(false);
				}
			}

			if(myBuilding == null) {

				// show buildings to select from
				renderedBuildings = new GameObject[buildings.Length];
				Debug.Log(numCanBeBuilt);
				Vector3 instantiatePosition = new Vector3(-numCanBeBuilt / 2 + .5f, 0, numCanBeBuilt / 2 - .5f);
				for(int i = 0; i < buildings.Length; i++) {
					if(canBeBuilt[i] && !(i == 0 && gameController.localityCreated)) {
						Debug.Log(instantiatePosition);
						renderedBuildings[i] = (GameObject)Instantiate(buildings[i], transform.position + instantiatePosition, Quaternion.identity);
						renderedBuildings[i].transform.parent = transform;
						instantiatePosition += new Vector3(1, -0.1f, -1);
					}
				}
			}

		// de-select
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

		// incremenet population if applicable
		if(myBuilding.countsForPopulation) {
			gameController.addPopulation(1);
		}

		// remove this building from renderedBuildings so that it does not get deleted
		for(int i = (gameController.localityCreated ? 1 : 0); i < buildings.Length; i++) {
			if(canBeBuilt[i]) {
				if(renderedBuildings[i].name.CompareTo(myBuilding.name) == 0) {
					renderedBuildings[i] = null;
				}
			}
		}

		// only allow one locality
		if(myBuilding.name == "Locality(Clone)") {
			gameController.localityCreated = true;;
		}

		// deselect this tile
		setSelected(false);
	}
}
