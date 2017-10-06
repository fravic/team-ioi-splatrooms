using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

	public float secondsInGame;
	public GameObject textObject;

	float _startTime;

	// Use this for initialization
	void Start () {
		StartTimer ();
	}

	// Update is called once per frame
	void Update () {
		float secondsLeft = secondsInGame - (Time.time - _startTime);
		System.TimeSpan t = System.TimeSpan.FromSeconds(secondsLeft);
		textObject.GetComponent<Text>().text = t.ToString ("mm\\:ss\\:ff");
	}

	public void StartTimer() {
		_startTime = Time.time;
	}
}
