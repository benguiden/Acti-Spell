using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour {

	public Transform reference;

	public float spawnWidth;

	private float lowestY;

	#region Platform Variables
	public Object prefab;

	public float spawnOffset;

	public float despawnOffset;

	public float xSpacing;

	public float ySpacing;

	public Level level;

	private float lastLevel;

	private float nextLevel;
	#endregion

	public BubbleGeneration bubbleGenScript;

	#region Mono Methods
	#if UNITY_EDITOR
	private void OnDrawGizmosSelected(){
		//Draw Spawn Width
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (this.transform.position, new Vector3 (spawnWidth, nextLevel, 0.1f));
	}
	#endif

	private void Start(){
		lowestY = reference.position.y;
		lastLevel = lowestY;
		nextLevel = lastLevel + Random.Range (level.levelSpacing.x, level.levelSpacing.y) - spawnOffset;
	}

	private void Update(){
		//Update Y Position
		if (reference.position.y > lowestY)
			lowestY = reference.position.y;

		while (lowestY >= nextLevel - spawnOffset) {
			GeneratePlatformLevel ();
		}

		//Remove Platforms
		RemovePreviousPlatforms();
	}
	#endregion

	#region Platform Generation
	private void RemovePreviousPlatforms(){
		for (int i = LevelController.main.GetPlatformCount () - 1; i >= 0; i--) {
			float yPos = LevelController.main.GetPlatform (i).transform.position.y;
			if (yPos <= reference.position.y + despawnOffset + level.yRange + ySpacing) {
				Destroy (LevelController.main.GetPlatform (i).gameObject);
			}
		}
	}

	private void GeneratePlatformLevel(){
		//Platform
		lastLevel = nextLevel;
		int platforms = (int)Mathf.Round(Random.Range (level.amount.x, level.amount.y));
		List<Vector2> newPlatformsPos = new List<Vector2> ();
		List<float> previousSpots = new List<float> (bubbleGenScript.platformSpots);
		for (int i = 0; i < previousSpots.Count; i++) {
			newPlatformsPos.Add (new Vector2 (previousSpots [i], -100f));
		}
		for (int i = 0; i < platforms; i++) {
			Vector2 platformPosition = new Vector2 ();
			platformPosition.x = Random.Range (-spawnWidth / 2f, spawnWidth / 2f);
			platformPosition.y = lastLevel + Random.Range (-level.yRange, level.yRange);

			SpawnPlatform (ref platformPosition, newPlatformsPos.ToArray ());
			newPlatformsPos.Add (platformPosition);
			previousSpots.Add (platformPosition.x);
		}
		if (previousSpots.Count > 3) {
			previousSpots.RemoveRange (0, previousSpots.Count - 3);
		}
		bubbleGenScript.platformSpots = previousSpots.ToArray ();
		nextLevel += Random.Range (level.levelSpacing.x, level.levelSpacing.y);

	}

	private void SpawnPlatform(ref Vector2 spawnPosition, Vector2[] prePlatformPos){
		GameObject platform = (GameObject)Instantiate (prefab, this.transform);
		#region X Spacing
		for (int i = 0; i < prePlatformPos.Length; i++) {
			float disBetween = Mathf.Abs (spawnPosition.x - prePlatformPos [i].x);
			if (disBetween < xSpacing) {
				if (spawnPosition.x < prePlatformPos [i].x)
					spawnPosition.x -= xSpacing - disBetween - Random.Range(0f, 0.5f);
				else
					spawnPosition.x += xSpacing - disBetween + Random.Range(0f, 0.5f);
			}
		}
		#endregion

		#region Y Spacing
		for (int i = 0; i < prePlatformPos.Length; i++) {
			float disBetween = Mathf.Abs (spawnPosition.y - prePlatformPos [i].y);
			if (disBetween < ySpacing) {
				if (spawnPosition.y < prePlatformPos [i].y)
					spawnPosition.y -= ySpacing - disBetween;
				else
					spawnPosition.y += ySpacing - disBetween;
			}
		}
		#endregion
		platform.transform.position = new Vector3 (spawnPosition.x, spawnPosition.y, 0f);

		//Check Wrapping
		float leftSide = platform.transform.position.x - (xSpacing / 2f) + (spawnWidth / 2f);
		float rightSide = platform.transform.position.x - (xSpacing / 2f) - (spawnWidth / 2f);
		if (leftSide < 0f) {
			((GameObject)Instantiate (prefab, this.transform)).transform.position = new Vector3 ((spawnWidth / 2f) + (xSpacing / 2f) + leftSide, spawnPosition.y, 0f);
		} else if (rightSide > -xSpacing) {
			((GameObject)Instantiate (prefab, this.transform)).transform.position = new Vector3 (-(spawnWidth / 2f) + (xSpacing / 2f) + rightSide, spawnPosition.y, 0f);
		}
	}
	#endregion

	[System.Serializable]
	public struct Level{
		public Vector2 amount;
		public Vector2 levelSpacing;
		public float yRange;
	}

}
