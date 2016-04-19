using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public Building[]	requiredNeighbors;
	public bool			placed;

	bool				highlighted;
	Transform			model;
	TileController		tile;

	void Start() {
		highlighted = false;
		model = transform.GetChild(0).transform;
		tile = GetComponentInParent<TileController>();
	}

	void Update() {
		if(!placed) {
			if(highlighted) {
				model.position = Vector3.Lerp(model.position, new Vector3(model.position.x, 1.2f, model.position.z), Time.deltaTime * 10);
			} else {
				model.position = Vector3.Lerp(model.position, new Vector3(model.position.x, 1f, model.position.z), Time.deltaTime * 5);
				// TODO: set origin points to center of models
				//model.Rotate(0, Time.deltaTime * 15, 0);
			}
		} else {
			// TODO: handle this in some other way because it is very inefficient
			transform.position = Vector3.Lerp(model.position, tile.transform.position, Time.deltaTime * 10);
			model.position = transform.position;
		}
	}

	void OnMouseOver() { highlighted = true; }
	void OnMouseExit() { highlighted = false; }

	void OnMouseDown() {
		if(!placed) {

			// place this building
			placed = true;
			tile.setBuilding(this);

			// deactivate box collider
			GetComponent<BoxCollider>().enabled = false;

			// TODO: lerp these values in a coroutine
			//transform.position = tile.transform.position;
			//model.position = tile.transform.position;
		}
	}
}
