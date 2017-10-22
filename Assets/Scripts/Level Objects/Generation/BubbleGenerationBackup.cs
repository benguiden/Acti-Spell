using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerationBackup : MonoBehaviour {

	public Object prefab;

	public int maxBubblesPerLevel;

	public float verticalSpacingMin;

	public float verticalSpacingMax;

	public Transform reference;

	public float referenceYOffset;

	public float despawnOffset;

	public float spawnWidth;

	private float lowestY;

	private float lastLevelY;

	private float nextLevelDistance;

	private void Start(){
		lowestY = reference.position.y;
		lastLevelY = reference.position.y + referenceYOffset;
		nextLevelDistance = Random.Range (verticalSpacingMin, verticalSpacingMax);
	}

	private void Update(){
		if (reference.position.y > lowestY) {
			lowestY = reference.position.y;
		}
		if (SpellingManager.main.state == SpellingManager.SpellingState.Spelling) {
			if (lowestY >= lastLevelY + nextLevelDistance + referenceYOffset) {
				//Generate
				GenerateNextLevel ();
			}
		} else {
			lastLevelY = reference.transform.position.y - referenceYOffset;
		}
	}

	private void GenerateNextLevel(){
		for (int i = LevelController.main.GetBubbleCount () - 1; i >= 0; i--) {
			float yPos = LevelController.main.GetBubble (i).transform.position.y;
			if (yPos <= reference.position.y + despawnOffset) {
				Destroy (LevelController.main.GetBubble (i).gameObject);
			}
		}
		lastLevelY += nextLevelDistance;
		int amount = Random.Range (1, maxBubblesPerLevel + 1);
		for (int i = 0; i < amount; i++) {
			SpawnBubble (Random.Range (-spawnWidth, spawnWidth));
		}
		nextLevelDistance = Random.Range (verticalSpacingMin, verticalSpacingMax);

	}

	private void SpawnBubble(float xPosition){
		GameObject platform = (GameObject)Instantiate (prefab, this.transform);
		platform.transform.position = new Vector3 (xPosition, lastLevelY, 0f);
	}

}
