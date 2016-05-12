using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {
	public Material		hoverMaterial,
						unsatisfiedMaterial;
	public GameObject[]	buildings,
						renderedBuildings;
	public GameObject	displayWhileSelected;
	public Building		myBuilding;
	public LayerMask	layerMaskTiles,
						layerMaskDefault;

	[HideInInspector]
	public int			ring;
	public bool[]		canBeBuilt;

	MeshRenderer		r;
	Material			defaultMaterial;
	bool				selected;
	RaycastHit[]		neighboringTiles;
	Building[]			neighboringBuildings;
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
	void OnMouseExit() { updateMaterial(); }
	public void updateMaterial() {
		if(myBuilding == null) {
			r.material = defaultMaterial;
		} else {
			r.material = myBuilding.satisfied ? defaultMaterial : unsatisfiedMaterial;
		}
	}

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

			// if we are not moving a building
			if(gameController.buildingToMove == null) {

				// de-select all other tiles
				foreach(TileController o in transform.parent.GetComponentsInChildren<TileController>()) {
					if(o != this) {
						o.setSelected(false);
					}
				}

				// if there isn't a building here yet
				if(myBuilding == null) {

					//TODO: fix that odd numbered sets of buildings offset??
					float weirdOffset = (numCanBeBuilt % 2 == 0) ? .5f : 0f;

					// show buildings to select from
					renderedBuildings = new GameObject[buildings.Length];
					//Debug.Log(numCanBeBuilt);
					Vector3 instantiatePosition = new Vector3(-numCanBeBuilt / 2 + weirdOffset, 0, numCanBeBuilt / 2 - weirdOffset);
					for(int i = 0; i < buildings.Length; i++) {
						if(canBeBuilt[i] && !(i == 0 && gameController.localityCreated)) {
							//Debug.Log(instantiatePosition);
							renderedBuildings[i] = (GameObject)Instantiate(buildings[i], transform.position + instantiatePosition, Quaternion.identity);
							renderedBuildings[i].transform.parent = transform;
							instantiatePosition += new Vector3(1, -0.1f, -1);
						}
					}
				}

				// if there is a building here
				else {

					// select this building to move it
					gameController.buildingToMove = myBuilding;
					//gameController.buildingToMove.held = true;
				}
			}

			// if we are moving a building
			else {

				// move building if the tile is free
				if(myBuilding == null) {
					
					// check if building could be placed here
					int indexOfBuildingToMove = 0;
					for(int i = 0; i < buildings.Length; i++) {
						if(buildings[i].name == gameController.buildingToMove.name.Substring(0, gameController.buildingToMove.name.Length - 7)) {
							indexOfBuildingToMove = i;
						}
					}

					// if it can be placed, do it!
					if(canBeBuilt[indexOfBuildingToMove]) {

						// remove building from its tile
						gameController.buildingToMove.transform.parent.GetComponent<TileController>().myBuilding = null;

						// give building to this tile
						myBuilding = gameController.buildingToMove;
						myBuilding.transform.SetParent(transform);

						// check for pollution
						bool chainWasBroken = false;
						foreach(GameObject o in GameObject.FindGameObjectsWithTag("Tile")) {

							TileController tile = o.GetComponent<TileController>();

							if(tile == null) {
								Debug.Log(o.name);
								o.name = "HELP_ME_SOMETHING_IS_VERY_VERY_WRONG";
								break;
							}

							if(tile.myBuilding != null) {

								// update buildings
								tile.updateBuildings();

								// check if chain was broken
								int indexOfMyBuilding = 0;
								for(int i = 0; i < buildings.Length; i++) {
									if(buildings[i].name == tile.myBuilding.name.Substring(0, tile.myBuilding.name.Length - 7)) {
										indexOfMyBuilding = i;
									}
								}
								if(!tile.canBeBuilt[indexOfMyBuilding] && tile.myBuilding.satisfied) {
									tile.myBuilding.satisfied = false;
									chainWasBroken = true;
								}

								// update material
								tile.updateMaterial();
							}
						}

						// pollute if chain was broken
						if(chainWasBroken) {
							gameController.pollute();
						}
					}

					// if it can't, gently apologize; it isn't the user's fault
					else {
						//TODO: visually show that this isn't a valid location
						Debug.Log("You can't put that there, dumbass!");
					}
				}

				// deselect all tiles
				foreach(TileController o in transform.parent.GetComponentsInChildren<TileController>()) {
					o.setSelected(false);
				}

			}
		}

		// de-select
		else {

			// no building is being moved
			//gameController.buildingToMove.held = false;
			gameController.buildingToMove = null;

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

	public void fallAway() {
		StartCoroutine("coFallAway");
	}
	public IEnumerator coFallAway() {
		GetComponent<BoxCollider>().enabled = false;
		float fallingSpeed = 0;
		while(transform.position.y > -10) {
			fallingSpeed += Time.deltaTime * 15;
			transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
		GameObject.Destroy(gameObject);
	}
}
