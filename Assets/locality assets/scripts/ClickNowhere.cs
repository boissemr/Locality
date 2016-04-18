using UnityEngine;
using System.Collections;

public class ClickNowhere : MonoBehaviour {

	void OnMouseDown() {
		
		// de-select all tiles
		foreach(TileController o in transform.parent.GetComponentsInChildren<TileController>()) {
			o.setSelected(false);
		}
	}
}
