using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	public char letter = ' ';

	public float radius = 0.5f;

	private TextMesh textMesh;

	[HideInInspector]
	public int bubbleIndex;

	private void Start(){
		if (this.gameObject.tag == "Bubble") {
			bubbleIndex = LevelController.main.GetBubbleCount ();
			LevelController.main.AddBubbleToList (this);
		} else {
			Debug.LogWarning ("Warning: Instance of Bubble with no 'Bubble' tag.");
		}
		letter = SpellingManager.main.GrabRandomLetter ();
		if (GetText ())
			textMesh.text = letter.ToString ();

	}

	private void OnDestroy(){
		if (this.gameObject.tag == "Bubble") {
			LevelController.main.RemoveBubbleFromList (bubbleIndex);
		}
	}

	private bool GetText(){
		if (textMesh == null) {
			textMesh = this.GetComponentInChildren<TextMesh> ();
			if (textMesh == null) {
				Debug.LogError ("Error: No TextMesh componenet in child Game Object of '" + this.gameObject.name + "' for Bubble component.");
				return false;
			}
			return true;
		}
		return true;
	}

}
