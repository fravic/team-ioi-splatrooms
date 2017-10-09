using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GameTimer : MonoBehaviour {

	public UnityEvent onTimerComplete;
	public float secondsInGame;
	public GameObject textObject;

	CanvasGroup _canvasGroup;
	bool _timerEnabled = false;
	bool _endEventFired = false;
	float _startTime;

	// Use this for initialization
	void Start () {
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	// Update is called once per frame
	void Update () {
		if (!_timerEnabled) {
			return;
		}

		float secondsLeft = Math.Max(secondsInGame - (Time.time - _startTime), 0);
		System.TimeSpan t = System.TimeSpan.FromSeconds(secondsLeft);
		textObject.GetComponent<Text>().text = t.ToString ("mm\\:ss");

		// Flash thrice every 10s or every second during the last 10
		int floorSec = Mathf.FloorToInt(secondsLeft);
		if (floorSec % 10 == 0 || floorSec % 10 == 9 || floorSec % 10 == 8 || floorSec < 10) {
			_canvasGroup.alpha = 1 - (Mathf.Ceil(secondsLeft) - secondsLeft);
		} else {
			_canvasGroup.alpha = 0;
		}

		if (secondsLeft <= 0 && !_endEventFired) {
			onTimerComplete.Invoke();
			_endEventFired = true;
		}
	}

	public void StartTimer() {
		_startTime = Time.time;
		_endEventFired = false;
		_timerEnabled = true;
	}
}
