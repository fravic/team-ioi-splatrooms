using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour {

	public UnityEvent onTimerComplete;
	public float secondsInGame;
	public GameObject textObject;

	bool _endEventFired = false;
	float _startTime;

	// Use this for initialization
	void Start () {
		StartTimer ();
	}

	// Update is called once per frame
	void Update () {
		float secondsLeft = Math.Max(secondsInGame - (Time.time - _startTime), 0);
		System.TimeSpan t = System.TimeSpan.FromSeconds(secondsLeft);
		textObject.GetComponent<Text>().text = t.ToString ("mm\\:ss\\:ff");

		if (secondsLeft <= 0 && !_endEventFired) {
			onTimerComplete.Invoke();
			_endEventFired = true;
		}
	}

	public void StartTimer() {
		_startTime = Time.time;
		_endEventFired = false;
	}
}
