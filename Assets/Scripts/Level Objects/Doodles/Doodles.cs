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
	public Object prefab;
	public float spawnOffset;
	public float despawnOffset;
	public Vector2 levelSpacing;

	[System.Serializable]
	public struct Theme
	{
		public string name;
		public Sprite[] sprites;
		public Sprite[] spritesColour;
	}

	#endregion

	#region Mono Methods

	private void Start ()
	{
		SpawnDoodle();
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
		GameObject doodle = (GameObject)Instantiate (prefab, this.transform);
		doodle.name = "Doodle";
		doodle.GetComponent<Doodle> ().parent = this;
	}

}
