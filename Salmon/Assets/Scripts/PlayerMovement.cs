using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

  public float Y_WATER_LEVEL;
  public float Y_MIN_OFFSET;
  public float Z_LEVEL;
  public float JUMP_MAX_HOLD_SECONDS;
  public float JUMP_SECONDS_TO_MAX_HEIGHT;
  public float JUMP_MAX_VELOCITY;
  public float JUMP_MIN_VELOCITY;
	public float MOVE_MAXSPEED;
  public float MOVE_SECONDS_TO_MAX_SPEED;
  public float MOVE_SECONDS_TO_STOP;
	public GameObject rail;
	
	// The time the jump button has been held down
	private float jumpHeldTime = 0;
  private float moveHeldTime = 0;
  private float moveStopTime = 0;
  private float moveStopSpeed = 0;
	private bool jumpingUp = false;
	private bool jumpStarted = false;
	private EasingFunction.Function JUMP_DOWN_EASE = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInQuad);
	private EasingFunction.Function JUMP_UP_EASE = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad);
  private EasingFunction.Function MOVE_EASE_IN = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad);
  private EasingFunction.Function MOVE_EASE_OUT = EasingFunction.GetEasingFunction(EasingFunction.Ease.Linear);

	// Use this for initialization
	void Start () {
		// hold a copy of the mother rail object for reference
		rail = GameObject.Find ("Rail");
	}
	
	// Update is called once per frame
	void Update()
	{
    // lock player movement onto forward and sideways axes
    Vector3 pos = transform.position;
    pos.z = rail.transform.position.z + Z_LEVEL;
    
    // Snap to water level
    if (!jumpStarted && pos.y < Y_WATER_LEVEL) {
      pos.y = Y_WATER_LEVEL;
    }
    
    // Physics!
    var rb = GetComponent<Rigidbody>();
    var velocity = rb.velocity;
    
    // Horizontal movement
    if (Input.GetAxis("Horizontal") != 0) {
    	moveStopTime = 0;
      moveHeldTime += Time.deltaTime;
      float percentHeld = Mathf.Clamp01(moveHeldTime / MOVE_SECONDS_TO_MAX_SPEED);
      velocity.x = MOVE_EASE_IN(0, MOVE_MAXSPEED, percentHeld);
      
      if (Input.GetAxis("Horizontal") < 0) {
      	velocity.x = -velocity.x;
      }
      moveStopSpeed = velocity.x;
    } else {
      moveHeldTime = 0;
    	moveStopTime += Time.deltaTime;
    	float percentHeld = Mathf.Clamp01(moveStopTime / MOVE_SECONDS_TO_STOP);
  		velocity.x = MOVE_EASE_OUT(0, moveStopSpeed, (1 - percentHeld));
    	
    	if (velocity.x == 0) {
    		moveStopSpeed = 0;
    	}
    }
    
    // Process jump
		if (jumpingUp) {
			// Going up
      
      // More gravity above water
      if (pos.y > Y_WATER_LEVEL) {
        rb.AddForce(0, Physics.gravity.y * 15, 0);
      }

			// Time to stop
			if (pos.y < Y_WATER_LEVEL && velocity.y < 0) {
				jumpHeldTime = 0;
				jumpingUp = false;
				jumpStarted = false;
				pos.y = Y_WATER_LEVEL;
			}
		} else {
			// Going down
			float percentHeld = 0;
			// Holding down button
			if (Input.GetButton("Jump")) {
				jumpStarted = true;
				jumpHeldTime += Time.deltaTime;
				percentHeld = Mathf.Clamp01(jumpHeldTime / JUMP_MAX_HOLD_SECONDS);
								
				pos.y = JUMP_DOWN_EASE(Y_WATER_LEVEL - Y_MIN_OFFSET, Y_WATER_LEVEL, (1 - percentHeld));
			}
			
			// Time to stop jumping
			if (Input.GetButtonUp("Jump")
			    || (jumpHeldTime > JUMP_MAX_HOLD_SECONDS)) {
				jumpingUp = true;
				
				percentHeld = Mathf.Clamp01(jumpHeldTime / JUMP_MAX_HOLD_SECONDS);
				velocity.y = JUMP_UP_EASE(JUMP_MIN_VELOCITY, JUMP_MAX_VELOCITY, percentHeld);
				// Debug.Log(velocity.y);
			}
		}
		rb.velocity = velocity;
		
		// Update position
		transform.position = pos;

		// Lock player rotation to nothing for right now
		Quaternion rot = transform.rotation;
		rot.x = 0;
		rot.y = 0;
		rot.z = 0;
		transform.rotation = rot;
	}
}
