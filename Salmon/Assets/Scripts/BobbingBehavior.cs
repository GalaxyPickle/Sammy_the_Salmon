using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingBehavior : MonoBehaviour {

	public int Y_WATER_LEVEL;
	public float BOB_RESET_GAIN;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

//		// Physics!
//		var rb = GetComponent<Rigidbody>();
//		var velocity = rb.velocity;
//
//		var max_velocity = rb.velocity.y;
//
//		if (max_velocity < BOB_RESET_GAIN) {
//			velocity.y += Mathf.Sign (velocity.y) * BOB_RESET_GAIN;
//		}
//		rb.velocity = velocity;
//
//		Debug.Log (rb.velocity);
	}
}
