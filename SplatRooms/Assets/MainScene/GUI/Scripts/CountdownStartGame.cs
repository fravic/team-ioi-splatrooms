// This C# function can be called by an Animation Event
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace MainScene {
  public class CountdownStartGame : MonoBehaviour {
    public UnityEvent startGameEvent;

    // Called from the Unity animation event
    public void StartGame() {
      startGameEvent.Invoke();
    }
  }
}