using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ShootData {
  public Vector3 position;
  public Vector3 direction;
}

/**
 * ShootHandler
 * Creates paint ball objects and gives them a velocity when the player shoots.
 */
public class ShootHandler : MonoBehaviour {
  public GameObject paintBallPrefab;

  private const float SHOOT_FORCE = 2.0f;
  private const float SHOOT_INTERVAL_S = 0.015f;

  private float _lastShotTime = 0.0f;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

  public void Shoot(ShootData shootData) {
    if (Time.time - _lastShotTime < SHOOT_INTERVAL_S) {
      return;
    }

    GameObject newPaintBall = Instantiate(paintBallPrefab);
    newPaintBall.transform.position = shootData.position;
    
    Rigidbody rb = newPaintBall.GetComponent<Rigidbody>();

    Vector3 startForce = shootData.direction * SHOOT_FORCE;
    rb.AddForce(startForce, ForceMode.Impulse);

    _lastShotTime = Time.time;
  }
}
