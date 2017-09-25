using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * SplatMaker
 * Detects collisions with Paintballs and creates splats from them.
 */
public class SplatMaker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision col) {
		if (col.gameObject.tag == "Paintball") {
			Destroy(col.gameObject);
		}
	}
}
