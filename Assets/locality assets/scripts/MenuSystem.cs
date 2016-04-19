using UnityEngine;
using System.Collections;

public class MenuSystem : MonoBehaviour {

	public MenuObject	title,
						mainMenu,
						tapToBeginText,
						titleBackgroundScene,
						gameplayArea,
						inGameUI;
	public GameObject	titleCamera,
						gameplayCamera,
						populationIndicatorPrefab,
						populationEmptyIndicatorPrefab;
	public GameController gameController;
	public SoundController soundController;
	public float		menuTransitionDuration,
						fadeToGameplayDuration;

	// called when screen tapped on in title screen
	public void tapToBegin() {

		// move title and main menu
		StartCoroutine(title.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(0.25f, 0.5f, 0)), menuTransitionDuration));
		StartCoroutine(mainMenu.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0)), menuTransitionDuration));

		// fade out "tap to begin"
		tapToBeginText.fadeTo(0, menuTransitionDuration);
		tapToBeginText.setRaycastTarget(false);
	}

	// called when new game is selected
	public void newGame() {

		// move title and main menu
		StartCoroutine(title.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(-1f, 0.5f, 0)), menuTransitionDuration));
		StartCoroutine(mainMenu.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(2f, 0.5f, 0)), menuTransitionDuration));

		// move out title background scene and move in gameplay scene
		StartCoroutine(titleBackgroundScene.moveTo(new Vector3(0, -30, 0), fadeToGameplayDuration));
		StartCoroutine(gameplayArea.moveTo(new Vector3(0, 0, 0), fadeToGameplayDuration / 2));

		// instantiate the first tile
		gameController.expand();

		// start the transition to gameplay
		StartCoroutine(instantiatePlayableArea(fadeToGameplayDuration));
	}

	IEnumerator instantiatePlayableArea(float secondsToWait) {

		// wait hold up for a sec
		yield return new WaitForSeconds(secondsToWait);

		// disable title scene and enable gameplay scene
		titleBackgroundScene.gameObject.SetActive(false);
		titleCamera.SetActive(false);
		gameplayCamera.SetActive(true);
		updatePopulationIndicator(0, 1);

		// fade in sunlight
		StartCoroutine(gameController.setSunlight(1f, secondsToWait));

		// switch to gameplay music
		soundController.playTrack(1);

		// move in-game UI
		StartCoroutine(inGameUI.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(0, 1f, 0)), menuTransitionDuration));
	}

	public void updatePopulationIndicator(int population, int populationForNextExpansion) {

		// clear population indicator
		for(int i = 0; i < inGameUI.transform.childCount; i++) {
			inGameUI.transform.GetChild(i).gameObject.SetActive(false);
		}

		// draw new population indicator
		for(int i = 0; i < populationForNextExpansion; i++) {

			// pick a sprite
			GameObject prefabToUse = (i >= population) ? populationEmptyIndicatorPrefab : populationIndicatorPrefab;

			// instantiate indicators
			GameObject temp = (GameObject)GameObject.Instantiate(prefabToUse, inGameUI.transform.position, Quaternion.identity);
			temp.transform.SetParent(inGameUI.transform);

			// align indicators
			temp.transform.position += new Vector3(40 * (i + 1), -50, 0);
		}
	}
}
