using UnityEngine;
using UnityEngine.UI;

/*
 * ScoreText
 * Sets the attached Text component to display the player's score.
 */
[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour {
  void Awake() {
    float score = Mathf.Round(PlayerPrefs.GetFloat("PlayerScore") * 100);
		GetComponent<Text>().text = score.ToString() + "%";
    Debug.Log("End Game Score: " + score);
  }
}
