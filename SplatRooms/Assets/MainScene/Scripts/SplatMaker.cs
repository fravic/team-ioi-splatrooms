using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * SplatMaker
 * Detects collisions with Paintballs and creates splats from them.
 */
public class SplatMaker : MonoBehaviour {

	// Just use one channel for now
	Vector4 channelMask = new Vector4(1, 0, 0, 0);

	float splatScale = 1.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision col) {
		foreach (ContactPoint contact in col.contacts) {
			MakeSplat(contact.point, contact.normal);
		}
		if (col.gameObject.tag == "Paintball") {
			Destroy(col.gameObject);
		}
	}

	// From Splatoonity/SplatMakerExample.cs
	public void MakeSplat(Vector3 point, Vector3 normal) {
		Vector3 leftVec = Vector3.Cross ( normal, Vector3.up );
		float randScale = Random.Range(0.5f,1.5f);
		
		GameObject newSplatObject = new GameObject();
		newSplatObject.transform.position = point;
		if ( leftVec.magnitude > 0.001f ){
			newSplatObject.transform.rotation = Quaternion.LookRotation( leftVec, normal );
		}
		newSplatObject.transform.RotateAround( point, normal, Random.Range(-180, 180 ) );
		newSplatObject.transform.localScale = new Vector3( randScale, randScale * 0.5f, randScale ) * splatScale;

		Matrix4x4 splatMatrix = newSplatObject.transform.worldToLocalMatrix;
		SplatManager sm = GetComponent<SplatManager>();
		sm.AddSplat(splatMatrix, channelMask);

		GameObject.Destroy( newSplatObject );
	}
}
