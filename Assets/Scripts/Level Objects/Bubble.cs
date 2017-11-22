using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour {

	public char letter = ' ';

	public float radius = 0.5f;

	public float jiggleAmount = 0.075f;
	public float jiggleSpeed = 0.1f;
	public float rotateAmount = 5f;
	public float rotatespeed = 10;
	Vector3 pos;

	private TextMesh textMesh;

	[HideInInspector]
	public int bubbleIndex;

	public void Initalize(){
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
	void Start(){
		pos = this.gameObject.transform.position;
	}
	void Update() {
		
		Vector3 newPos = pos;
		newPos.x += Mathf.PingPong (Time.time * jiggleSpeed, jiggleAmount);
		transform.localEulerAngles = new Vector3(0, 0, -Mathf.PingPong(Time.time * rotatespeed, rotateAmount)); 
		transform.position = newPos;

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
