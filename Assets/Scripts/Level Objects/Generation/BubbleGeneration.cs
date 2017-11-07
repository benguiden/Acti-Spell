using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BubbleGeneration : MonoBehaviour {

	#region Variables & Objects
	#region Reference
	public Transform reference;
	private float lowestY;
	private float lastLevelY;
	private float nextLevelY;
	#endregion

	#region Spawning
	public float spawnWidth;
	public Object prefab;
	public float spawnOffset;
	public float despawnOffset;
	public float radius;
	public float zPosition;
	public Level level;
	#endregion

	[HideInInspector]
	public float[] platformSpots;

	[System.Serializable]
	public struct Level{
		public Vector2 amountPerLevel;
		public Vector2 levelSpacing;
	}

	#endregion

	#region Mono Methods
	#if UNITY_EDITOR
	private void OnDrawGizmosSelected(){
		//Draw Spawn Width
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (this.transform.position, new Vector3 (spawnWidth, 0.1f, 0.1f));
	}
	#endif

	private void Start(){
		lowestY = reference.position.y;
		lastLevelY = reference.position.y + spawnOffset;
		nextLevelY = lastLevelY + Random.Range (level.levelSpacing.x, level.levelSpacing.y);
		platformSpots = new float[0];
	}

	private void Update(){
		//Update Y Position
		if (reference.position.y > lowestY)
			lowestY = reference.position.y;

		//Generate Bubbles
		if ((SpellingManager.main.state == SpellingManager.SpellingState.Spelling) || (SpellingManager.main.state == SpellingManager.SpellingState.DespawningWord)) {
			if (lowestY >= nextLevelY - spawnOffset) {
				GenerateBubbles ();
			}
		} else {
			nextLevelY = reference.transform.position.y + spawnOffset;
		}

	}

	#endregion

	#region Bubble Generation
	private void GenerateBubbles(){
		//remove Previous Bubbles
		RemoveBubbles ();

		//Update Spawning Properties
		lastLevelY = nextLevelY;
		int spawnAmount = Random.Range ((int)Mathf.Round (level.amountPerLevel.x), (int)Mathf.Round (level.amountPerLevel.y) + 1);
		List<Vector2> bubblePositions = new List<Vector2> ();

		//Spawn New Bubbles
		for (int i = 0; i < spawnAmount; i++) {
			float yOffset = Random.Range (0.25f, 1f);
			//Decide Spawn Position
			float bubbleXPosition;
			if (platformSpots.Length < 1) {
				bubbleXPosition = Random.Range (-(spawnWidth / 2f) + radius, (spawnWidth / 2f) - radius);
			} else {
				int platform = Random.Range (0, platformSpots.Length);
				bubbleXPosition = platformSpots [platform];
				bubbleXPosition += Random.Range (-1f, 1f);
				bubbleXPosition = Mathf.Clamp (bubbleXPosition, -(spawnWidth / 2f) + radius, (spawnWidth / 2f) - radius);
			}

			if (SpawnBubble (ref bubbleXPosition, lastLevelY + (spawnOffset / 2f) + yOffset, bubblePositions.ToArray ())) {
				bubblePositions.Add (new Vector2 (bubbleXPosition, lastLevelY + yOffset + (spawnOffset / 2f)));
			}
		}

		//Set Next Level
		nextLevelY += Random.Range(level.levelSpacing.x, level.levelSpacing.y);

	}

	private bool SpawnBubble(ref float xPosition, float yPosition, Vector2[] bubblePositions){
		GameObject bubble = (GameObject)Instantiate (prefab, this.transform);
		bubble.GetComponent<Bubble> ().Initalize ();

		//Spacing
		for (int i = 0; i < bubblePositions.Length; i++) {
			float disBetween = Mathf.Abs (xPosition - bubblePositions [i].x);
			if (disBetween < (radius * 2.5f)) {
				if (xPosition < bubblePositions [i].x){
					xPosition -= (radius * 2.5f) + disBetween;
					if (xPosition < -spawnWidth / 2f) {
						Destroy (bubble);
						return false;
					}
				}
				else {
					xPosition += (radius * 2.5f) + disBetween;
					if (xPosition > spawnWidth / 2f) {
						Destroy (bubble);
						return false;
					}
				}
			}
		}

		//Set Position
		bubble.transform.position = new Vector3(xPosition, yPosition, zPosition);

		return true;
	}

	private void RemoveBubbles(){
		for (int i = LevelController.main.GetBubbleCount () - 1; i >= 0; i--) {
			float yPosition = LevelController.main.GetBubble (i).transform.position.y;
			if (yPosition <= reference.position.y + despawnOffset + (radius * 2f)) {
				Destroy (LevelController.main.GetBubble (i).gameObject);
			}
		}
	}
	#endregion

}
