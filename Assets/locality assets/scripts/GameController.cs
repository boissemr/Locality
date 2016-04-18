using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject	tilePrefab;
	public Light		sunlight;
	public int			population;
	public float		tileSeparation;

	int					populationForNextExpansion,
						expansionLevel;

	void Start() {
		populationForNextExpansion = 0;
		expansionLevel = 0;
	}

	void Update() {

		// debug add population
		// TODO: remove this before publishing
		if(Input.GetButtonDown("DebugAddPopulation")) {
			addPopulation(1);
		}
	}

	public void addPopulation(int amount) {

		// add amount
		population += amount;

		// is it enough to expand?
		if(population >= populationForNextExpansion) {
			expand();
		}
	}

	public void expand() {

		// increment expansion level
		expansionLevel++;

		// log
		Debug.Log("EXPANSION: level " + expansionLevel + ", population " + population);

		// first tile
		if(expansionLevel == 1) {
			GameObject child = (GameObject)Instantiate(tilePrefab, transform.position, Quaternion.identity);
			child.transform.parent = transform;
		}

		// typical expansion after first
		else {
			
			// iterate over 4 sides
			for(int side = 0; side < 4; side++) {
				
				// set offset distance from center
				Vector3 offset = new Vector3(1 + tileSeparation, 0, 0) * (expansionLevel - 1);

				// set offset shift according to how many tiles there will be
				offset -= new Vector3(0, 0, (expansionLevel - 1) + (expansionLevel - 1) * tileSeparation);

				// set offset direction from center
				Quaternion direction = Quaternion.Euler(0, side * 90, 0);

				// iterate over n tiles per side
				for(int tile = 0; tile < (expansionLevel - 1) * 2; tile++) {

					// instantiate a tile
					GameObject child = (GameObject)Instantiate(tilePrefab, transform.position + direction * offset, Quaternion.identity);
					child.transform.parent = transform;

					// increase shift as tiles are created
					offset += new Vector3(0, 0, 1 + tileSeparation);
				}
			}
		}

		// set population for next expansion
		// next expansion will cost 1 more than this one did
		populationForNextExpansion += expansionLevel;
	}

	public IEnumerator setSunlight(float value, float duration) {
		while(Mathf.Abs(sunlight.intensity - value) > .01f) {
			sunlight.intensity = Mathf.Lerp(sunlight.intensity, value, Time.deltaTime / duration);
			yield return new WaitForEndOfFrame();
		}
	}
}
