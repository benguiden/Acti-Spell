using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGenerationBackup : MonoBehaviour {

	public Section stats; /*Change name in future*/

	public Object prefab;

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
		nextLevelDistance = Random.Range (stats.verticalSpacingMin, stats.verticalSpacingMax);
	}

	private void Update(){
		if (reference.position.y > lowestY) {
			lowestY = reference.position.y;
		}

		while (lowestY >= lastLevelY + nextLevelDistance + referenceYOffset) {
			//Generate
			GenerateNextLevel();
		}
	}

	private void GenerateNextLevel(){
		for (int i = LevelController.main.GetPlatformCount () - 1; i >= 0; i--) {
			float yPos = LevelController.main.GetPlatform (i).transform.position.y;
			if (yPos <= reference.position.y + despawnOffset) {
				Destroy (LevelController.main.GetPlatform (i).gameObject);
			}
		}
		lastLevelY += nextLevelDistance;
		int amount = Random.Range (1, stats.horizontalSegments + 1);
		for (int i = 0; i < amount; i++) {
			SpawnPlatform (Random.Range (-spawnWidth, spawnWidth));
		}
		nextLevelDistance = Random.Range (stats.verticalSpacingMin, stats.verticalSpacingMax);

	}

	private void SpawnPlatform(float xPosition){
		GameObject platform = (GameObject)Instantiate (prefab, this.transform);
		platform.transform.position = new Vector3 (xPosition, lastLevelY, 0f);
	}

	[System.Serializable]
	public struct Section{
		public int horizontalSegments;
		public float verticalSpacingMin;
		public float verticalSpacingMax;
	}

}
