using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMovement : MonoBehaviour {

	public int railspeed = 10;
	public bool move = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.Space)) {
			move = !move;
		}
		if (move) {
			transform.Translate (Vector3.forward * railspeed * Time.deltaTime);
		}
	}
}
