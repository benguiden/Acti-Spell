using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Font3D : MonoBehaviour {

	#region Variables & Objects
	//Visible
	public bool setStatic;
	public Sprite[] _letterSprites;
	public float _letterScale;
	public float _letterGap;

	//Static
	public static Sprite[] letterSprites;
	public static float letterScale;
	public static float letterGap;

	//Private
	private string text;
	#endregion

	#region Mono Methods
	private void Start(){
		if (setStatic) {
			letterSprites = _letterSprites;
			if (letterSprites.Length != 26)
				Debug.LogError ("Error: Letter Sprites array length is not 26.");
			letterScale = _letterScale;
			letterGap = _letterGap;
			Destroy (this.gameObject);
		}
	}
	#endregion

	#region Text Methods
	public void SetText(string newString){
		text = newString;
		//Delete Previous Children
		foreach (Transform child in transform){
			Destroy (child.gameObject);
		}

		//Make Sprite Array out of String
		char[] letters = newString.ToLower().ToCharArray();
		int[] spriteIndexes = new int[letters.Length];
		float canvasWidth = 0f;
		for (int i = 0; i < letters.Length; i++) {
			spriteIndexes [i] = GetLetterIndex (letters [i]);
			if (i != letters.Length - 1)
				canvasWidth += (letterSprites [spriteIndexes[i]].bounds.size.x + letterGap) * letterScale;
			else
				canvasWidth += letterSprites [spriteIndexes[i]].bounds.size.x * letterScale;
		}

		//Set Letters
		float xPos = ((letterSprites [spriteIndexes [0]].bounds.size.x / 2f) * letterScale) - (canvasWidth / 2f);
		for (int i = 0; i < spriteIndexes.Length; i++) {
			SetLetter (xPos, letterScale, spriteIndexes [i]);
			xPos += (letterSprites [spriteIndexes [i]].bounds.size.x / 2f * letterScale);
			xPos += (letterGap * letterScale);
			if (i < spriteIndexes.Length - 1)
				xPos += (letterSprites [spriteIndexes [i + 1]].bounds.size.x / 2f * letterScale);
		}

	}

	private void SetLetter(float xPos, float scale, int spriteIndex){
		GameObject letter = new GameObject ("Letter");
		letter.transform.parent = this.transform;
		letter.transform.localPosition = new Vector3 (xPos, 0f, 0f);
		letter.transform.localScale = new Vector3 (scale, scale, scale);
		letter.AddComponent<SpriteRenderer> ().sprite = letterSprites [spriteIndex];
	}

	private int GetLetterIndex(char letter){
		return ((int)letter) - 97;
	}

	public string GetText(){
		return text;
	}
	#endregion

}
