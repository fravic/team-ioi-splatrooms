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

	public Camera debugCamera;
	public GameObject shootSpawnPoint;
	public ShootEvent shootEvent;

	bool _shootingEnabled = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!_shootingEnabled) {
			return;
		}

		// Oculus right hand trigger
		if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) {
			Vector3 localPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTrackedRemote);
			Vector3 localDir = Vector3.right;

			ShootData data;
			data.position = shootSpawnPoint.transform.TransformPoint(localPos);
			data.direction = shootSpawnPoint.transform.TransformDirection(localDir);
			shootEvent.Invoke(data);
		}

		// Also accept mouse events for debugging
		// Raycasting from the VR camera does not work very well due to VR distortion
		// See https://forum.unity.com/threads/screenpointtoray-and-viewportpointtoray-not-working-with-vr.471440/
		if (Input.GetMouseButton(0)) {
			Vector3 mousePos = Input.mousePosition;
			float normalWidth = mousePos.x / Screen.width;
			float normalHeight = mousePos.y / Screen.height;
			Vector3 viewMousePos = new Vector3(normalWidth, normalHeight, 0);
			Ray ray = debugCamera.ViewportPointToRay(viewMousePos);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 100)) {
				Debug.DrawRay(ray.origin, ray.direction, Color.red);
				SplatMaker sm = hit.collider.gameObject.GetComponent<SplatMaker>();
				if (sm) {
					sm.MakeSplat(hit.point, hit.normal);
				}
			}
		}
	}

	public void EnableShooting() {
		_shootingEnabled = true;
	}
}
