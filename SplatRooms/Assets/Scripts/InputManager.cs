using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class ShootEvent : UnityEvent<ShootData> {
}

/**
 * InputManager
 * Gets input from the Oculus Touch controllers and fires events accordingly.
 */
public class InputManager : MonoBehaviour {

	public GameObject shootSpawnPoint;
	public ShootEvent shootEvent;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) {
			Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote);
			Vector3 localDir = Vector3.right;

			ShootData data;
			data.position = shootSpawnPoint.transform.TransformPoint(localPos);
			data.direction = shootSpawnPoint.transform.TransformDirection(localDir);
			shootEvent.Invoke(data);
		}
	}
}
