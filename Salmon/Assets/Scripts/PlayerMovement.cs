using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public int MOVESPEED;
	public int Y_MAX;
	public int Y_LEVEL;
	public int Y_MIN;
	public int Z_LEVEL;
	public GameObject rail;

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
		pos.y = Y_LEVEL;
		pos.z = rail.transform.position.z + Z_LEVEL;
		
		// Process jump
		if (Input.GetButton("Jump")) {
			pos.y = Y_MIN;
		} else if (Input.GetButtonUp("Jump")) {
			pos.y = Y_MAX;
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
