using UnityEngine;
using System.Collections;

public class MenuSystem : MonoBehaviour {

	public MenuObject	title,
						mainMenu,
						tapToBeginText,
						titleBackdrop;
	public float		menuTransitionDuration;

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
	}
}
