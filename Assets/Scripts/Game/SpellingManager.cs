﻿using System.Collections;
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

	public TextAsset libraryFile;
	public string[,] library;
	private List<string> currentWordGroup;
	private int[] rowLength;

	private string currentWord;
	private int currentWordIndex;
	private char[] currentWordLetters;

	[HideInInspector]
	public SpellingState state = SpellingState.SpawningWord;

	public enum SpellingState{Spelling,SpawningWord,DespawningWord}

	#region Mono Methods
	private void Awake(){
		InitalizeLibrary ();
	}

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

	private void InitalizeLibrary(){
		//To long string
		string textFromFile = libraryFile.text.Replace ("\n", "");

		//To rows of strings
		string[] rows = textFromFile.Split (';');
		rowLength = new int[rows.Length];
		int wordCount = 0;

		for (int i = 0; i < rows.Length - 1; i++) {
			string[] words = rows [i].Split (',');
			if (words.Length > wordCount)
				wordCount = words.Length;
			rowLength [i] = words.Length;
		}

		//Cast To Library
		library = new string[rows.Length - 1, wordCount];
		for (int r = 0; r < rows.Length - 1; r++) {
			string[] words = rows [r].Split (',');
			for (int w = 0; w < wordCount; w++) {
				if (w < rowLength [r])
					library [r, w] = words [w].ToUpper ();
				else
					library [r, w] = "";
			}
		}
	}

	private void NewWordGroup(){
		int level = Score.main.level;
		currentWordGroup = new List<string> ();
		for (int i = 0; i < rowLength [level]; i++) {
			currentWordGroup.Add (library [level, i]);
		}
	}

	private string NewWord(){
		StartCoroutine (ScribbleWord (1f));
		currentSpelling = "";
		currentLetterIndex = 0;
		currentWordIndex = Random.Range (0, currentWordGroup.Count);
		currentWord = currentWordGroup [currentWordIndex];
		if (currentWordIndex == 0) {
			string newWord = currentWord.Remove (0, 1);
			currentWordLetters = newWord.ToCharArray ();
		} else {
			currentWordLetters = currentWord.ToCharArray ();
		}
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

			if (Score.main.nextLevelReady) {
				NewWordGroup ();
				Score.main.nextLevelReady = false;
			}

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

				if (Score.main.nextLevelReady) {
					NewWordGroup ();
					Score.main.nextLevelReady = false;
				}

				NewWord ();
				LevelController.main.RemoveBubbles ();
				state = SpellingState.SpawningWord;
				Score.main.CheckWordCount (true);
			}
				
		}
	}

}
