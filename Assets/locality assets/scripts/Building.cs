using UnityEngine;
using System.Collections;

public class Building : MonoBehaviour {

	public Building[]	requiredNeighbors;
	public bool			placed,
						countsForPopulation,
						held;

	float				highlightedHeight = 1.4f,
						unhighlightedHeight = 1.0f,
						placedHeight = .05f;
	bool				highlighted;
	Transform			model;
	TileController		tile;

	void Start() {
		highlighted = false;
		model = transform.GetChild(0).transform;
		tile = GetComponentInParent<TileController>();
	}

	void Update() {
		if(!placed || held) {
			if(highlighted) {
				model.position = Vector3.Lerp(model.position, new Vector3(model.position.x, highlightedHeight, model.position.z), Time.deltaTime * 10);
				model.Rotate(0, Time.deltaTime * 90, 0);
			} else {
				model.position = Vector3.Lerp(model.position, new Vector3(model.position.x, unhighlightedHeight, model.position.z), Time.deltaTime * 5);
				model.rotation = Quaternion.Lerp(model.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 5);
			}
		} else {
			// TODO: handle this in a coroutine because it is very inefficient
			transform.position = Vector3.Lerp(model.position, transform.parent.transform.position + Vector3.up * placedHeight, Time.deltaTime * 10);
			model.position = transform.position;
			model.rotation = Quaternion.Lerp(model.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 10);
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
