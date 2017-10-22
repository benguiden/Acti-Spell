using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doodles : MonoBehaviour
{

	#region Variables & Object

	[Header ("Themes")]
	public string currentTheme;
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
	}

	private float highestY;

	#endregion

	#region Mono Methods

	private void Start ()
	{
		reference = Camera.main.transform;
		highestY = 0f;
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
		if (themes != null) {
			foreach (Theme theme in themes) {
				if (currentTheme.ToLower () == theme.name.ToLower ()) {
					if (isColour) {
						int sprIndex = Random.Range (0, theme.spritesColour.Length);
						return theme.spritesColour [sprIndex];
					} else {
						int sprIndex = Random.Range (0, theme.sprites.Length);
						return theme.sprites [sprIndex];
					}
				}
			}
		}
		return null;
	}

	public void SpawnDoodle(){
		GameObject doodle = new GameObject ("Doodle");
		doodle.transform.parent = this.transform;
		doodle.AddComponent<Doodle> ().parent = this;
	}

}
