using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellingManager : MonoBehaviour {

	public static SpellingManager main;

	public TextMesh displayText;

	#region Word To Spell Variables
	public Object wordPrefab;
	public Transform wordSpawnReference;
	public float wordSpawnOffset, wordInitalY; //Positive
	private GameObject spawnedWord;

	private AudioSource audioSource;
	public AudioClip correct;
	public Object correctSpellingPrefab;
	#endregion

	public string currentSpelling;
	private int currentLetterIndex;

	public string[] library;
	private List<string> currentWordGroup;

	private string currentWord;
	private int currentWordIndex;
	private char[] currentWordLetters;

	[HideInInspector]
	public SpellingState state = SpellingState.SpawningWord;

	public enum SpellingState{Spelling,SpawningWord,DespawningWord}

	#region Mono Methods
	private void Start(){
		audioSource = gameObject.GetComponent<AudioSource> ();
		main = this;
		currentSpelling = "";
		state = SpellingState.SpawningWord;
		NewWordGroup ();
	}

	private void Update(){
		//Check state
		switch (state) {

		case SpellingState.SpawningWord:
			if (wordSpawnReference.position.y - wordInitalY >= wordSpawnOffset) {
				SpawnNewWord ();
			}
			break;

		case SpellingState.DespawningWord:
			if (spawnedWord.transform.position.y < wordSpawnReference.position.y - (wordSpawnOffset * 2)) {
				Destroy (spawnedWord);
				state = SpellingState.Spelling;
			}
			break;

		//Rememebr to set wordInitalY;
		case SpellingState.Spelling:
			break;
		}
	}
	#endregion

	private void NewWordGroup(){
		currentWordGroup = new List<string> (library);
	}

	private string NewWord(){
		StartCoroutine (ScribbleWord (1f));
		currentSpelling = "";
		currentLetterIndex = 0;
		currentWordIndex = Random.Range (0, currentWordGroup.Count);
		currentWord = currentWordGroup [currentWordIndex];
		currentWordLetters = currentWord.ToCharArray ();
		return currentWord;
	}

	private IEnumerator ScribbleWord(float time){
		while (time > 0f) {
			time -= Time.deltaTime;
			yield return null;
		}
		displayText.text = "";
	}

	private void SpawnNewWord(){
		if (spawnedWord != null)
			Destroy (spawnedWord);
		spawnedWord = (GameObject)Instantiate (wordPrefab, this.transform);
		spawnedWord.name = "New Word";
		Vector3 newPos = spawnedWord.transform.position;
		newPos.y = wordSpawnReference.position.y + wordSpawnOffset;
		spawnedWord.transform.position = newPos;
		spawnedWord.GetComponentInChildren<Font3D> ().SetText (NewWord ());
		state = SpellingState.DespawningWord;
	}

	public char GrabRandomLetter(){
		if (Random.Range (0, 3) == 0) {
			int letterIndex = currentLetterIndex;
			if (Random.Range (0, 3) == 0)
				letterIndex = Random.Range (0, currentWordLetters.Length);
			return currentWordLetters [letterIndex];
		} else {
			if (Random.Range (0, 3) == 0)
				return currentWordLetters [currentLetterIndex];
			return (char)(Random.Range (65, 91));
		}
	}

	public void AddLetter(char letter){
		currentSpelling += letter.ToString ();
		displayText.text = currentSpelling;
		if (letter != currentWordLetters [currentLetterIndex]) {
			//Wrong word
			Debug.Log("Incorrect!");
			NewWord ();
			LevelController.main.RemoveBubbles ();
			state = SpellingState.SpawningWord;
			Score.main.RestartWordMultiplier ();
		} else {
			currentLetterIndex++;
			if (currentWordLetters.Length == currentLetterIndex) {
				//Right word
				Debug.Log("Correct!");
				audioSource.PlayOneShot (correct);
				((GameObject)Instantiate (correctSpellingPrefab, Camera.main.transform)).transform.localPosition = new Vector3 (-2.9f, -5f, 10f);

				NewWord ();
				LevelController.main.RemoveBubbles ();
				state = SpellingState.SpawningWord;
				Score.main.CheckWordCount (true);
			}
				
		}
	}

}
