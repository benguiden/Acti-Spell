﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellingManager : MonoBehaviour {

	public static SpellingManager main;

	public TextMesh displayText;

	public AnimationCurve fontSizeCurve;

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

	public BubbleGeneration bubbleGen;

	public float[] lettersWidth;
	public float fontSizeK;

	private string currentWord;
	private int currentWordIndex;
	private char[] currentWordLetters;

	private bool lastLetter = false;

	private int newFontSize = 0;

	[HideInInspector]
	public SpellingState state = SpellingState.SpawningWord;

	public enum SpellingState{Spelling,SpawningWord,DespawningWord,WaitingToDespawn}

	#region Mono Methods
	private void Awake(){
		InitalizeLibrary ();
		main = this;
		currentSpelling = "";
		state = SpellingState.SpawningWord;
	}

	private void Start(){
		audioSource = gameObject.GetComponent<AudioSource> ();
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
			if (spawnedWord.transform.position.y < wordSpawnReference.position.y - (wordSpawnOffset)) {
				if (spawnedWord.activeSelf) {
					spawnedWord.SetActive (false);
					LevelController.main.DestroyLater (spawnedWord);
				}
				state = SpellingState.Spelling;

			} else if ((spawnedWord.transform.position.y < wordSpawnReference.position.y) /*&& (!bubbleGen.isSpawning)*/) {
				bubbleGen.isSpawning = true;
			}
			break;

		case SpellingState.WaitingToDespawn:
			bubbleGen.isSpawning = false;
			if (spawnedWord == null)
				state = SpellingState.SpawningWord;
			else if (spawnedWord.transform.position.y < wordSpawnReference.position.y - (wordSpawnOffset)) {
				if (spawnedWord.activeSelf) {
					spawnedWord.SetActive (false);
					LevelController.main.DestroyLater (spawnedWord);
				}
				state = SpellingState.SpawningWord;
			}
			break;

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
		if (Score.main.isCapped)
			level = Random.Range (0, Score.main.level + 1);
		currentWordGroup = new List<string> ();
		for (int i = 0; i < rowLength [level]; i++) {
			currentWordGroup.Add (library [level, i]);
		}
	}

	private string NewWord(){
		if (currentWordGroup.Count < 1) {
			NewWordGroup ();
		}

		currentSpelling = "";
		currentLetterIndex = 0;
		PlayerController.Pitch = 0.3f;
		currentWordIndex = Random.Range (0, currentWordGroup.Count);
		currentWord = currentWordGroup [currentWordIndex];
		if ((currentWordIndex == 0) && (Score.main.level > 0)) {
			string newWord = currentWord.Remove (0, 1);
			currentWordLetters = newWord.ToCharArray ();
		} else {
			currentWordLetters = currentWord.ToCharArray ();
		}

		CalculateFontSize (currentWord);
		return currentWord;
	}

	private void SpawnNewWord(){
		if (spawnedWord != null) {
			if (spawnedWord.activeSelf) {
				spawnedWord.SetActive (false);
				LevelController.main.DestroyLater (spawnedWord);
			}
		}
		spawnedWord = (GameObject)Instantiate (wordPrefab, this.transform);
		Vector3 newPos = spawnedWord.transform.position;
		newPos.y = wordSpawnReference.position.y + wordSpawnOffset;
		spawnedWord.transform.position = newPos;

		spawnedWord.GetComponentInChildren<Font3D> ().SetText (NewWord ());
		state = SpellingState.DespawningWord;
	}

	public char GrabRandomLetter(){
		int correctPosibility = 0;
		if (lastLetter)
			correctPosibility = -1;
		if (Random.Range (0, 3) == correctPosibility) {
			int letterIndex = currentLetterIndex;
			lastLetter = true;
			if (Random.Range (0, 3) == 0) {
				letterIndex = Random.Range (0, currentWordLetters.Length);
				lastLetter = false;
			}
			return currentWordLetters [letterIndex];
		} else {
			lastLetter = false;
			return (char)(Random.Range (65, 91));
		}
	}

	public void AddLetter(char letter, Bubble poppedBubble){
		currentSpelling += letter.ToString ();
		displayText.text = currentSpelling;
		if (letter != currentWordLetters [currentLetterIndex]) {
			//Wrong word
			PlayerController.Pitch = 0.1f;
			if ((Score.main.nextLevelReady) && (!Score.main.isCapped)) {
				NewWordGroup ();
				Score.main.nextLevelReady = false;
			}else if (Score.main.isCapped)
				NewWordGroup ();

			//Change bubble to red
			SpriteRenderer bubbleRen = poppedBubble.GetComponent<SpriteRenderer>();
			if (bubbleRen != null)
				bubbleRen.color = Color.red;
			TextMesh bubbleText = poppedBubble.GetComponentInChildren<TextMesh>();
			if (bubbleText != null)
				bubbleText.color = Color.red;

			bubbleGen.isSpawning = false;
			state = SpellingState.WaitingToDespawn;
			CrossOut.main.CrossWordOut (currentWord, displayText.fontSize);
			LevelController.main.RemoveBubbles ();
			Score.main.RestartWordMultiplier ();
		} else {
			currentLetterIndex++;
			if (currentWordLetters.Length == currentLetterIndex) {
				//Right word
				audioSource.PlayOneShot (correct);
				Instantiate (correctSpellingPrefab, Camera.main.transform);
				if (Score.main.nextLevelReady) {
					NewWordGroup ();
					Score.main.nextLevelReady = false;
				}
				bubbleGen.isSpawning = false;
				StartCoroutine (CorrectSpellingTimer ());
				LevelController.main.RemoveBubbles ();
				state = SpellingState.WaitingToDespawn;
				Score.main.CheckWordCount (true);
				if (currentWordIndex < currentWordGroup.Count)
					currentWordGroup.RemoveAt (currentWordIndex);
				else
					Debug.LogError ("Error to report to Ben: Word index greater than word list length. Index: " + currentWordIndex.ToString () + "   List Length: " + currentWordGroup.Count);
			}
				
		}
	}

	private IEnumerator CorrectSpellingTimer(){
		yield return new WaitForSeconds (0.8f);
		float deleteLetterTime = 0.8f / (float)displayText.text.Length;
		int index = displayText.text.Length;
		float time = 0f;
		while (index > 0) {
			if (time <= 0f) {
				index--;
				time += deleteLetterTime;
				displayText.text = displayText.text.Remove (displayText.text.Length - 1);
			}

			time -= Time.deltaTime;
			yield return null;
		}

		displayText.text = "";
		ApplyNewFontSize ();
	}

	private void CalculateFontSize(string word){
		if (word.Length > 0) {
			word = word.ToUpper ();
			char[] characters = word.ToCharArray ();
			float wordWidth = 0f;
			for (int i = 0; i < characters.Length; i++) {
				wordWidth += lettersWidth [((int)characters [i]) - 65];
			}
			newFontSize = (int)(fontSizeCurve.Evaluate ((float)characters.Length) * (fontSizeK * (float)characters.Length / wordWidth));
			newFontSize = Mathf.Clamp (newFontSize, 0, 75);
		}
	}

	public void ApplyNewFontSize(){
		displayText.fontSize = newFontSize;
	}

}
