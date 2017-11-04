using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerController : MonoBehaviour {

	//Reminder!!//
	///////Jump buffer after falling////////

	#region public
	public Vector2 bounds;

	public Vector2 boundsOffset;

	public Vector2 bubbleBounds;

	public Vector2 bubbleBoundsOffset;

	public float horizontalSpeed = 4f;

	public float jumpTime;
	
	public float gravity;

	public float jumpHeight;

	public float wrapWidth = 10f;

	public float jumpSpd;
	public float jumpBoost;

	public Vector2 velocity;

	public Object dust;
	public bool landed = false;

	//Audio
	public static float Pitch = 0.5f; 
	public AudioClip bubblePop;
	public AudioClip stomp;
	public AudioClip flipSound;
	public AudioClip jumpSound;
	AudioSource sound;
	public bool turning = false;

	Animator anim;

	#endregion

	#region private
	private Vector2 accelerationBoundsSize;
	private Vector2 accelerationBoundsOffset;
	private bool grounded = false;
	private bool groundedLast = false;
	private bool started = false;
	private Vector2 lastVelocity;
	private float lastDeltaTime;

	//Componenets
	private SpriteRenderer childRen;
	//private GameObject child;

	#endregion

	#if UNITY_EDITOR
	private void OnDrawGizmosSelected(){
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube (this.transform.position, new Vector3 (wrapWidth * 2f, 0.1f, 0.1f));
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube (this.transform.position + (Vector3)accelerationBoundsOffset, new Vector3 (accelerationBoundsSize.x, accelerationBoundsSize.y, 0.1f));
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube (this.transform.position + (Vector3)bubbleBoundsOffset, new Vector3 (bubbleBounds.x, bubbleBounds.y, 0.1f));
	}
	#endif

	#region GameStates
	private void Start(){
		sound = gameObject.GetComponent<AudioSource> ();
		anim = gameObject.GetComponent<Animator> ();
		SetJumpHeight (jumpHeight);
		childRen = this.GetComponentInChildren<SpriteRenderer> ();
		/*if (childRen == null)
			Debug.LogError ("Error: No Sprite Renderer componenet in child Game Object of '" + this.gameObject.name + "' with Player Controller componenet.");
		else
			child = childRen.gameObject;*/
	}

	private void OnValidate(){
		SetJumpHeight (jumpHeight);
	}

	private void Update(){
		SetBounds ();
		Move ();
		Wrap ();

		if (velocity.y <= 0f)
			CheckCollisions ();

		CheckBubbleCollisions ();

		lastVelocity = velocity;
		lastDeltaTime = Time.deltaTime;

	}
	#endregion

	#region Movement
	private void Move(){

		///////Jump buffer after falling////////

		//Move Left & Right
		if ((Input.GetKey (KeyCode.RightArrow)) && !(Input.GetKey (KeyCode.LeftArrow))) {
			velocity.x = horizontalSpeed;
			if (grounded) {
				anim.SetBool ("Walk", true);
			}
			childRen.flipX = false;
			if (!sound.isPlaying && turning) {
				sound.PlayOneShot (flipSound);
				turning = false;
			}
		} else if (!(Input.GetKey (KeyCode.RightArrow)) && (Input.GetKey (KeyCode.LeftArrow))) {
			velocity.x = -horizontalSpeed;
			if (grounded) {
				anim.SetBool ("Walk", true);
			}
			childRen.flipX = true;
			if (!sound.isPlaying && !turning) {
				sound.PlayOneShot (flipSound);
				turning = true;
			}
		} else {
			velocity.x = 0f;
			anim.SetBool ("Walk", false);
		}

		if (started)
			velocity.y -= Mathf.Abs (gravity * Time.deltaTime);

		//Jump
		if ((Input.GetKey (KeyCode.UpArrow)) && (grounded)) {
			started = true;
			velocity.y = Mathf.Abs (jumpSpd);
			anim.SetBool ("jumping", true);
			sound.pitch = (Random.Range (0.8f, 1.3f));
			sound.PlayOneShot (jumpSound);
			landed = true;
		}

		grounded = false;

		if ((Input.GetKey (KeyCode.UpArrow)) && (velocity.y > 0)) {
			jumpBoost -= Time.deltaTime;
			velocity.y += jumpBoost * Time.deltaTime;
		}

		this.transform.Translate ((Vector3)velocity * Time.deltaTime); // * timeSpeed

	}

	private void Wrap(){
		Vector3 pos = this.transform.position;
		if (pos.x > wrapWidth)
			pos.x = -wrapWidth;
		else if (pos.x < -wrapWidth)
			pos.x = wrapWidth;
		this.transform.position = pos;
	}

	public void SetJumpHeight(float height){
		jumpHeight = height;
		jumpSpd = 2f * height / jumpTime;
		gravity = 2f * height / Mathf.Pow (jumpTime, 2f);
	}

	public float GetJumpHeight(){
		return jumpHeight;
	}

	public bool IsGrounded(){
		return grounded;
	}
	#endregion

	#region Collisions
	private void CheckCollisions(){
		Platform[] platforms = LevelController.main.GetPlatforms ();
		for (int i = 0; i < platforms.Length; i++) {
			if (platforms [i] != null) {
				if (CheckCollision (platforms [i])) {
					//Collision!
					Vector3 pos = this.transform.position;
					this.transform.position = new Vector3 (pos.x, platforms [i].BoundsYToPosition (1f)/* + (bounds.y / 2f) - boundsOffset.y*/, pos.z);
					velocity.y = 0f;
					if (groundedLast == false)
						platforms [i].Bob ();
					grounded = true;
					if (landed) {
						GameObject speck = ((GameObject)Instantiate (dust, platforms [i].transform, false));
						Destroy (speck, 1f);
						landed = false;
					}
					anim.SetBool ("jumping", false);
					jumpBoost = gravity * (jumpTime * 2);
				}
			} else {
				Debug.LogWarning ("Attention: Trying to access destroyed platform.");
			}
		}
		groundedLast = grounded;
	}

	private bool CheckCollision(Platform platform){
		if (BoundsXToPosition (0f) <= platform.BoundsXToPosition (1f)) {//Left of player < right of platform
			if (BoundsXToPosition (1f) >= platform.BoundsXToPosition (0f)) {//Right of player > left of platform
				if (BoundsYToPosition (0f) <= platform.BoundsYToPosition (1f)) {//Bottom of player < top of platform
					if (BoundsYToPosition (1f) >= platform.BoundsYToPosition (0f)) {//Top of player > bottom of platform
						return true;
					} else
						return false;
				} else
					return false;
			} else
				return false;
		} else
			return false;
	}

	private void CheckBubbleCollisions(){
		Bubble[] bubbles = LevelController.main.GetBubbles ();
		for (int i = bubbles.Length - 1; i >= 0; i--) {
			if (CheckBubbleCollision (bubbles [i]) && bubbles [i].enabled == true) {
				//Collision!
				SpellingManager.main.AddLetter(bubbles[i].letter);
				Pitch += 0.15f;
				sound.pitch = Pitch;
				sound.PlayOneShot (bubblePop);
				Animator bubAnim = bubbles[i].GetComponent<Animator> ();
				bubAnim.SetTrigger ("collected");
				LevelController.main.GetBubble (i).gameObject.GetComponent<Bubble> ().enabled = false;
				Destroy (LevelController.main.GetBubble(i).gameObject, 0.5f);
			}
		}
	}

	private bool CheckBubbleCollision(Bubble bubble){
		Vector2 closestPoint = new Vector2 ();
		Vector2 bubblePos = (Vector2)bubble.transform.position;

		#region Get Closest Point
		float boxX0 = BubbleBoundsXToPosition (0f);
		float boxX1 = BubbleBoundsXToPosition (1f);
		if (bubblePos.x <= boxX0)
			closestPoint.x = boxX0;
		else if (bubblePos.x >= boxX1)
			closestPoint.x = boxX1;
		else
			closestPoint.x = bubblePos.x;

		float boxY0 = BubbleBoundsYToPosition (0f);
		float boxY1 = BubbleBoundsYToPosition (1f);
		if (bubblePos.y <= boxY0)
			closestPoint.y = boxY0;
		else if (bubblePos.y >= boxY1)
			closestPoint.y = boxY1;
		else
			closestPoint.y = bubblePos.y;
		#endregion

		if (Vector2.Distance (closestPoint, bubblePos) <= bubble.radius) {
			return true;
		}
		return false;
	}

	#endregion

	#region Bounds
	private void SetBounds(){
		accelerationBoundsSize = bounds + (new Vector2 (Mathf.Abs (velocity.x), Mathf.Abs (velocity.y)) * Time.deltaTime) + (new Vector2 (Mathf.Abs (lastVelocity.x), Mathf.Abs (lastVelocity.y)) * lastDeltaTime);
		accelerationBoundsOffset = boundsOffset + (velocity * Time.deltaTime) - (lastVelocity * lastDeltaTime);
	}

	public Vector2 BoundsToPosition(Vector2 boundPos){
		boundPos.x = Mathf.Clamp01 (boundPos.x);
		boundPos.y = Mathf.Clamp01 (boundPos.y);
		float xPosition = this.transform.position.x - (accelerationBoundsSize.x / 2f) + (accelerationBoundsSize.x * boundPos.x) + accelerationBoundsOffset.x;
		float yPosition = this.transform.position.y - (accelerationBoundsSize.y / 2f) + (accelerationBoundsSize.y * boundPos.y) + accelerationBoundsOffset.y;
		return new Vector2 (xPosition, yPosition);
	}

	public float BoundsXToPosition(float x){
		x = Mathf.Clamp01 (x);
		return this.transform.position.x - (accelerationBoundsSize.x / 2f) + (accelerationBoundsSize.x * x) + accelerationBoundsOffset.x;
	}

	public float BoundsYToPosition(float y){
		y = Mathf.Clamp01 (y);
		return this.transform.position.y - (accelerationBoundsSize.y / 2f) + (accelerationBoundsSize.y * y) + accelerationBoundsOffset.y;
	}

	public Vector2 BubbleBoundsToPosition(Vector2 boundPos){
		boundPos.x = Mathf.Clamp01 (boundPos.x);
		boundPos.y = Mathf.Clamp01 (boundPos.y);
		float xPosition = this.transform.position.x - (bubbleBounds.x / 2f) + (bubbleBounds.x * boundPos.x) + bubbleBoundsOffset.x;
		float yPosition = this.transform.position.y - (bubbleBounds.y / 2f) + (bubbleBounds.y * boundPos.y) + bubbleBoundsOffset.y;
		return new Vector2 (xPosition, yPosition);
	}

	public float BubbleBoundsXToPosition(float x){
		x = Mathf.Clamp01 (x);
		return this.transform.position.x - (bubbleBounds.x / 2f) + (bubbleBounds.x * x) + bubbleBoundsOffset.x;
	}

	public float BubbleBoundsYToPosition(float y){
		y = Mathf.Clamp01 (y);
		return this.transform.position.y - (bubbleBounds.y / 2f) + (bubbleBounds.y * y) + bubbleBoundsOffset.y;
	}
	#endregion
}
