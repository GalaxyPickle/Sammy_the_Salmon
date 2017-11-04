using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public int MOVESPEED;
	public float Y_MAX_OFFSET;
	public float Y_WATER_LEVEL;
	public float Y_MIN_OFFSET;
	public float Z_LEVEL;
	public float JUMP_MAX_HOLD_SECONDS;
	public float JUMP_SECONDS_TO_MAX_HEIGHT;
	public GameObject rail;
	
	// The time the jump button has been held down
	private float jumpHeldTime = 0;
	private float jumpAirElapsed = 0;
	private bool jumpingUp = false;
	private bool jumpStarted = false;

	// Use this for initialization
	void Start () {
		// hold a copy of the mother rail object for reference
		rail = GameObject.Find ("Rail");
	}
	
	// Update is called once per frame
	void Update()
	{
		var x = Input.GetAxis("Horizontal") * Time.deltaTime * MOVESPEED;
		transform.Translate(x, 0, 0);

		// lock player movement onto forward and sideways axes
		Vector3 pos = transform.position;
		pos.z = rail.transform.position.z + Z_LEVEL;
		
		// Snap to water level
		if (!jumpStarted && pos.y < Y_WATER_LEVEL) {
			pos.y = Y_WATER_LEVEL;
		}
		
		// Process jump
		if (jumpingUp) {
			// Going up
			jumpAirElapsed += Time.deltaTime;
			float percentHeld = jumpHeldTime / JUMP_MAX_HOLD_SECONDS;
			float maxTime = percentHeld * JUMP_SECONDS_TO_MAX_HEIGHT;
			
			// Time to stop
			if (jumpAirElapsed > maxTime) {
				jumpAirElapsed = 0;
				jumpHeldTime = 0;
				jumpingUp = false;
				jumpStarted = false;
			}
		} else {
			// Going down
			float percentHeld = 0;
			// Holding down button
			if (Input.GetButton("Jump")) {
				jumpStarted = true;
				jumpHeldTime += Time.deltaTime;
				percentHeld = jumpHeldTime / JUMP_MAX_HOLD_SECONDS;
				pos.y = Y_WATER_LEVEL - Y_MIN_OFFSET * percentHeld;
			}
			
			// Time to stop jumping
			if (Input.GetButtonUp("Jump")
			    || (jumpHeldTime > JUMP_MAX_HOLD_SECONDS)) {
				jumpingUp = true;
				
				// Set velocity y = 1/2*a*t^2 +v0*t + y0
				// v0 = (y - 1/2*a*t^2 - y0)/t
				var rb = GetComponent<Rigidbody>();
				var velocity = rb.velocity;
				percentHeld = jumpHeldTime / JUMP_MAX_HOLD_SECONDS;
				float maxHeight = percentHeld * Y_MAX_OFFSET;
				float maxTime = percentHeld * JUMP_SECONDS_TO_MAX_HEIGHT;
				velocity.y = ((maxHeight + Y_WATER_LEVEL)
				                 -0.5f*Physics.gravity.y*maxTime*maxTime
				                 -pos.y)/maxTime;
				rb.velocity = velocity;
			}
		}
		
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
