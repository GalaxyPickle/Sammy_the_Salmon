﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public AudioClip splash_submerge;
	public AudioClip splash_jump;
	public AudioClip splash_land;
	public AudioClip splat;
	public AudioSource audiosource;

	public ParticleSystem particle_system_splash;
	public ParticleSystem particle_system_water_movement;

	public float Y_WATER_LEVEL;
	public float Y_MIN_OFFSET;
	public float Z_LEVEL;
	public float X_MIN;
	public float X_MAX;
	public float JUMP_MAX_HOLD_SECONDS;
	public float JUMP_SECONDS_TO_MAX_HEIGHT;
	public float JUMP_MAX_VELOCITY;
	public float JUMP_MIN_VELOCITY;
	public float MOVE_MAXSPEED;
	public float MOVE_SECONDS_TO_MAX_SPEED;
	public float MOVE_SECONDS_TO_STOP;
	public float MOVE_SPEED_PERCENT_ABOVE_WATER;
	public GameObject rail;
	
	// The time the jump button has been held down
	private float jumpHeldTime = 0;
	private float moveHeldTime = 0;
	private float moveStopTime = 0;
	private float moveStopSpeed = 0;
	private bool jumpingUp = false;
	private bool jumpStarted = false;
	private bool aboveWater = false;
	private bool underWater = false;
	private EasingFunction.Function JUMP_DOWN_EASE = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseInQuad);
	private EasingFunction.Function JUMP_UP_EASE = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad);
	private EasingFunction.Function MOVE_EASE_IN = EasingFunction.GetEasingFunction(EasingFunction.Ease.EaseOutQuad);
	private EasingFunction.Function MOVE_EASE_OUT = EasingFunction.GetEasingFunction(EasingFunction.Ease.Linear);

	// Use this for initialization
	void Start () {
		// hold a copy of the mother rail object for reference
		rail = GameObject.Find ("Rail");

		audiosource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update()
	{
	    // lock player movement onto forward and sideways axes
	    Vector3 pos = transform.position;
	    pos.z = rail.transform.position.z + Z_LEVEL;
	    
	    if (pos.y < Y_WATER_LEVEL) {
		    underWater = true;
		    aboveWater = false;
	    } else if (pos.y > Y_WATER_LEVEL) {
	    	underWater = false;
		    aboveWater = true;
	    } else {
	    	underWater = false;
		    aboveWater = false;
	    }
	    
	    // Snap to water level
	    if (!jumpStarted && underWater) {
	      pos.y = Y_WATER_LEVEL;
	    }
	    
	    // Physics!
	    var rb = GetComponent<Rigidbody>();
	    var velocity = rb.velocity;
	    
	    // Horizontal movement
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) {
			moveStopTime = 0;
			moveHeldTime += Time.deltaTime;
			float percentHeld = Mathf.Clamp01(moveHeldTime / MOVE_SECONDS_TO_MAX_SPEED);

			if (aboveWater && percentHeld > MOVE_SPEED_PERCENT_ABOVE_WATER) {
				percentHeld = MOVE_SPEED_PERCENT_ABOVE_WATER;
			}
			velocity.x = MOVE_MAXSPEED;

			if (Input.GetKey(KeyCode.LeftArrow)) {
				velocity.x = -velocity.x;
			}
			moveStopSpeed = velocity.x;

			// play swim effects
			if (!aboveWater) {
				// swim sound
				if (!audiosource.isPlaying) {
					audiosource.PlayOneShot (splash_submerge);
				}
				// play particles
//				particle_system_water_movement.Play();
			}
	    } 
		else {
			moveHeldTime = 0;
			moveStopTime += Time.deltaTime;
			float percentHeld = Mathf.Clamp01(moveStopTime / MOVE_SECONDS_TO_STOP);
			velocity.x = MOVE_EASE_OUT(0, moveStopSpeed, (1-percentHeld));
			velocity.x = 0;
			if (velocity.x == 0) {
				moveStopSpeed = 0;
			}
	    }
		//Debug.Log (Input.GetAxis ("Horizontal"));
    
	    // Clamp to sides
	    pos.x = Mathf.Clamp(pos.x, X_MIN, X_MAX);
	    
	    // Process jump
		if (jumpingUp) {
			// Going up
      
			// More gravity above water
			if (aboveWater) {
				rb.AddForce(0, Physics.gravity.y * 15, 0);
			}

			// Time to stop
			if (underWater && velocity.y < 0) {
				jumpHeldTime = 0;
				jumpingUp = false;
				jumpStarted = false;
				pos.y = Y_WATER_LEVEL;

				// EMIT PARTICLES!!!
				particle_system_splash.Play();

				audiosource.PlayOneShot (splash_land);
			}
		} 
		else {
			// Going down
			float percentHeld = 0;
			// Holding down button
			if (Input.GetButton("Jump")) {
				// play jump sound!
				if (!jumpStarted)
					audiosource.PlayOneShot(splash_submerge, 2);

				jumpStarted = true;
				jumpHeldTime += Time.deltaTime;
				percentHeld = Mathf.Clamp01(jumpHeldTime / JUMP_MAX_HOLD_SECONDS);
								
				pos.y = JUMP_DOWN_EASE(Y_WATER_LEVEL - Y_MIN_OFFSET, Y_WATER_LEVEL, (1 - percentHeld));
			}
			
			// Time to stop jumping
			if (Input.GetButtonUp("Jump")
			    || (jumpHeldTime > JUMP_MAX_HOLD_SECONDS)) {

				// play land sound?
				audiosource.PlayOneShot(splash_jump);

				jumpingUp = true;
				
				percentHeld = Mathf.Clamp01(jumpHeldTime / JUMP_MAX_HOLD_SECONDS);
				velocity.y = JUMP_UP_EASE(JUMP_MIN_VELOCITY, JUMP_MAX_VELOCITY, percentHeld);
				// Debug.Log(velocity.y);

				// PARTICLES!!!
				particle_system_splash.Play();
			}
		}
		rb.velocity = velocity;
		
		// Update position
		transform.position = pos;

		// player rotation depends on velocity
		Quaternion rot = transform.rotation;
		rot.x = velocity.x;
		rot.y = velocity.y;
		rot.z = velocity.z;
		transform.rotation = rot;

		// track particle system to player position
		particle_system_splash.transform.position = transform.position;
		particle_system_water_movement.transform.position = transform.position;

		// stop particles if out of water
		if (aboveWater) {
			// stop particle system for movement
//			particle_system_water_movement.Stop();
		}

		// reset level
		//if (Input.GetButtonDown("Cancel"));
	}

	void OnCollisionEnter (Collision col) {
		if (col.gameObject.name == "obstacle") {
			Application.LoadLevel (0);
			audiosource.PlayOneShot (splat);
		}
	}
}
