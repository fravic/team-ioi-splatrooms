using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * ScoreText
 * Sets the attached Text component to display the player's score.
 */
[RequireComponent(typeof(Text))]
public class ScoreText : MonoBehaviour {
  float _scoreDisplay;

  void Awake() {
    float score = PlayerPrefs.GetFloat("PlayerScore");
    //StartCoroutine(AnimateScore(score));
    StartCoroutine(AnimateScore(.9f));
  }

  IEnumerator AnimateScore(float score) {
    while (_scoreDisplay < score) {
      yield return new WaitForSeconds(0.02f);
      _scoreDisplay = Mathf.Min(_scoreDisplay + 0.01f, score);

      int scoreText = Mathf.RoundToInt(_scoreDisplay * 100);
      GetComponent<Text>().text = scoreText.ToString() + "%";
    }
  }
}
