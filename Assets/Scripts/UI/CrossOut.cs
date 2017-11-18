using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossOut : MonoBehaviour {

	#region Public Variables
	public Object crossOutPrefab, correctSpellingPrefab;
	public float crossoutTime;
	public float correctSpellTime;
	public float crossSize;

	public static CrossOut main;
	#endregion


	#region Mono Methods
	private void Awake(){
		main = this;
	}
	#endregion


	#region Public Method
	public void CrossWordOut(string correctWord, int fontSize){
		StartCoroutine (IECrossWordOut (correctWord, fontSize));
	}

	private IEnumerator IECrossWordOut(string correctWord, int fontSize){
		Transform line = ((GameObject)Instantiate (crossOutPrefab, this.transform)).transform;
		float time = crossoutTime;
		while (time > 0f) {
			Vector3 lineScale = line.localScale;
			lineScale.x = crossSize * (1f - (time / crossoutTime));
			line.localScale = lineScale;
			time -= Time.deltaTime;
			yield return null;
		}
		TextMesh correctText = ((GameObject)Instantiate (correctSpellingPrefab, this.transform)).GetComponent<TextMesh> ();
		correctText.fontSize = fontSize;
		int letterIndex = 0;
		char[] correctLetters = correctWord.ToCharArray ();
		while (letterIndex < correctLetters.Length) {
			correctText.text += correctLetters [letterIndex];
			time = correctSpellTime / (float)correctLetters.Length;
			while (time > 0f) {
				time -= Time.deltaTime;
				yield return null;
			}
			letterIndex++;
		}
		yield return new WaitForSeconds (1f);
		SpellingManager.main.displayText.text = "";
		Destroy (line.gameObject);
		Destroy (correctText.gameObject);
	}
	#endregion

}
