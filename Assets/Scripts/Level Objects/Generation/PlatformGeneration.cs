using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformGeneration : MonoBehaviour {

	public Transform reference;

	public float spawnWidth;

	public float lowestPosition;

	public float lowestY;

	public float zPos;

	#region Platform Variables
	public Object prefab;

	public float spawnOffset;

	public float despawnOffset;

	public float xSpacing;

	public float ySpacing;

	public Level level;

	private float lastLevel;

	private float nextLevel;

	private bool state = true;
	#endregion

	public BubbleGeneration bubbleGenScript;

	public int colourIndex;

	public Color[] colours;

	#region Mono Methods
	#if UNITY_EDITOR
	private void OnDrawGizmosSelected(){
		//Draw Spawn Width
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (new Vector3(0f, reference.position.y + despawnOffset + level.yRange + ySpacing, 0f), new Vector3 (spawnWidth, 0.1f, 0.1f));
	}
	#endif

	private void Start(){
		lowestY = reference.position.y;
		lastLevel = lowestPosition + spawnOffset;
		nextLevel = lastLevel + Random.Range (level.levelSpacing.x, level.levelSpacing.y) - spawnOffset;
	}

	private void Update(){
		//Update Y Position
		if (reference.position.y > lowestY)
			lowestY = reference.position.y;

		if (state == true) {
			while (lowestY >= nextLevel - spawnOffset) {
				GeneratePlatformLevel ();
			}
			state = false;
		} else {
			//Remove Platforms
			RemovePreviousPlatforms ();
			state = true;
		}
	}
	#endregion

	#region Platform Generation
	private void RemovePreviousPlatforms(){
		for (int i = LevelController.main.GetPlatformCount () - 1; i >= 0; i--) {
			float yPos = LevelController.main.GetPlatform (i).transform.position.y;
			if (yPos <= reference.position.y + despawnOffset + level.yRange + ySpacing) {
				if (LevelController.main.GetPlatform (i).gameObject.activeSelf) {
					LevelController.main.GetPlatform (i).gameObject.SetActive (false);
					LevelController.main.DestroyLater (LevelController.main.GetPlatform (i).gameObject);
				}
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
		platform.GetComponent<SpriteRenderer> ().color = colours [colourIndex];

		#region Spacing
		for (int j = 0; j<2; j++){
			for (int i = 0; i < prePlatformPos.Length; i++) {
				Vector2 disBetween = new Vector2();
				disBetween.x = Mathf.Abs (spawnPosition.x - prePlatformPos [i].x);
				disBetween.y = Mathf.Abs (spawnPosition.y - prePlatformPos [i].y);
				if ((disBetween.x < xSpacing) && (disBetween.y < ySpacing)){
					Debug.Log(platform.GetInstanceID().ToString() + " " + prePlatformPos [i].x.ToString() + " " + j.ToString());
					//Spread X
					if (j ==0){
						if (spawnPosition.x < prePlatformPos [i].x)
							spawnPosition.x -= xSpacing - disBetween.x  + Random.Range(0.05f, 0.5f);
						else
							spawnPosition.x += xSpacing - disBetween.x + Random.Range(0.05f, 0.5f);
					}else{ //Spread Y
						if (spawnPosition.y < prePlatformPos [i].y)
							spawnPosition.y -= ySpacing - disBetween.y + Random.Range(0.05f, 0.5f);
						else
							spawnPosition.y += ySpacing - disBetween.y + Random.Range(0.05f, 0.5f);
					}
				}
			}
		}
		#endregion
		platform.transform.position = new Vector3 (spawnPosition.x, spawnPosition.y, 0f);

		//Check Wrapping
		float leftSide = platform.transform.position.x - (xSpacing / 2f) + (spawnWidth / 2f);
		float rightSide = platform.transform.position.x - (xSpacing / 2f) - (spawnWidth / 2f);
		Transform platformTrans = null;
		if (leftSide < 0f) {
			platformTrans = ((GameObject)Instantiate (prefab, this.transform)).transform;
			platformTrans.position = new Vector3 ((spawnWidth / 2f) + (xSpacing / 2f) + leftSide, spawnPosition.y, zPos);
		} else if (rightSide > -xSpacing) {
			platformTrans = ((GameObject)Instantiate (prefab, this.transform)).transform;
			platformTrans.position = new Vector3 (-(spawnWidth / 2f) + (xSpacing / 2f) + rightSide, spawnPosition.y, zPos);
		}
		if (Score.main.isCapped)
			colourIndex = Random.Range (0, colours.Length);
		if (platformTrans != null)
			platformTrans.GetComponent<SpriteRenderer> ().color = platform.GetComponent<SpriteRenderer> ().color;
	}
	#endregion

	[System.Serializable]
	public struct Level{
		public Vector2 amount;
		public Vector2 levelSpacing;
		public float yRange;
	}

	public void IncreaseIndex(){
		colourIndex++;
		if (colourIndex >= colours.Length)
			colourIndex = 0;
	}

}
