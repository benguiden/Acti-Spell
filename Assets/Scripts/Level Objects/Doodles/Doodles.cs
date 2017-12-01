using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doodles : MonoBehaviour
{

	#region Variables & Object

	[Header ("Themes")]
	public int themeIndex;
	public Theme[] themes;

	[Space]
	[Header ("Randomness")]
	public float rotationAmount;
	public Vector2 scale;
	public float offsetAmount;

	[Space]
	[Header ("Spawning")]
	public float spawnWidth;
	public Transform reference;
	public float spawnOffset;
	public float despawnOffset;
	public float levelSpacing;

	[Space]
	[Header ("Look")]
	public float parallaxAmount;

	[System.Serializable]
	public struct Theme
	{
		public string name;
		public Sprite[] sprites;
		public Sprite[] spritesColour;
		public AnimatedDoodle[] animated;
	}

	[System.Serializable]
	public struct AnimatedDoodle
	{
		public Sprite[] sprites;
		public float fps;
	}

	private float highestY = 0f;

	private List<Vector2> doodlePositions, doodleSizes;

	#endregion

	#region Mono Methods

	private void Start ()
	{
		reference = Camera.main.transform;
		doodlePositions = new List<Vector2> ();
		doodleSizes = new List<Vector2> ();
		SpawnDoodle();
	}

	private void Update(){
		while (reference.position.y - highestY >= levelSpacing) {
			highestY += levelSpacing;
			SpawnDoodle ();
			if (Random.Range(0f, 1f) < 0.25f)
				SpawnDoodle ();
		}
	}

	#endregion

	public Sprite GetSprite (bool isColour)
	{
		if (Score.main.isCapped)
			themeIndex = Random.Range (0, themes.Length);
		isColour = true;
		if (Random.value <= 0.3f)
			isColour = false;
		if (themes != null) {
			Theme theme = themes [themeIndex];
			if (isColour) {
				int sprIndex = Random.Range (0, theme.spritesColour.Length);
				return theme.spritesColour [sprIndex];
			} else {
				int sprIndex = Random.Range (0, theme.sprites.Length);
				return theme.sprites [sprIndex];
			}
		}
		return null;
	}

	public AnimatedDoodle GetAnimated(){
		if (Score.main.isCapped)
			themeIndex = Random.Range (0, themes.Length);
		Theme theme = themes [themeIndex];
		if (theme.animated.Length == 0) {
			Debug.LogError ("Error: No animated doodles in theme.");
			Debug.Break ();
			return theme.animated [0];
		} else {
			return theme.animated [Random.Range (0, theme.animated.Length)];
		}
			
	}

	public void SpawnDoodle(){
		GameObject doodle = new GameObject ("Doodle");
		doodle.transform.parent = this.transform;
		doodle.AddComponent<Doodle> ().parent = this;
	}

	public void IncreaseIndex(){
		themeIndex++;
		if (themeIndex >= themes.Length)
			themeIndex = 0;
	}

	public Vector2[] GetDoodlePositions(){
		return doodlePositions.ToArray ();
	}

	public void AddDoodlePosition(Vector2 newPosition){
		doodlePositions.Add (newPosition);
		if (doodlePositions.Count > 4)
			doodlePositions.RemoveRange (0, doodlePositions.Count - 4);
	}

	public Vector2[] GetDoodleSizes(){
		return doodleSizes.ToArray ();
	}

	public void AddDoodleSize(Vector2 newSize){
		doodleSizes.Add (newSize);
		if (doodleSizes.Count > 4)
			doodleSizes.RemoveRange (0, doodleSizes.Count - 4);
	}

}
