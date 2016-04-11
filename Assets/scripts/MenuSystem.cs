using UnityEngine;
using System.Collections;

public class MenuSystem : MonoBehaviour {

	public MenuObject	title,
						mainMenu,
						tapToBeginText,
						titleBackdrop,
						titleBackgroundScene,
						gameplayArea;
	public GameObject	titleCamera,
						gameplayCamera;
	public GameController gameController;
	public float		menuTransitionDuration,
						fadeToGameplayDuration;

	// called when screen tapped on in title screen
	public void tapToBegin() {
		StartCoroutine(title.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(0.25f, 0.5f, 0)), menuTransitionDuration));
		StartCoroutine(mainMenu.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0)), menuTransitionDuration));
		tapToBeginText.fadeTo(0, menuTransitionDuration);
		tapToBeginText.setRaycastTarget(false);
	}

	// called when new game is selected
	public void newGame() {
		titleBackdrop.fadeTo(0, menuTransitionDuration);
		StartCoroutine(title.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(-1f, 0.5f, 0)), menuTransitionDuration));
		StartCoroutine(mainMenu.moveTo(Camera.main.ViewportToScreenPoint(new Vector3(2f, 0.5f, 0)), menuTransitionDuration));
		StartCoroutine(titleBackgroundScene.moveTo(new Vector3(0, -100, 0), fadeToGameplayDuration));
		StartCoroutine(instantiatePlayableArea(fadeToGameplayDuration));
	}

	IEnumerator instantiatePlayableArea(float secondsToWait) {
		
		StartCoroutine(gameplayArea.moveTo(new Vector3(0, 0, 0), secondsToWait));
		
		yield return new WaitForSeconds(secondsToWait);

		titleBackgroundScene.gameObject.SetActive(false);
		gameController.expand();
		titleCamera.SetActive(false);
		gameplayCamera.SetActive(true);
	}
}
