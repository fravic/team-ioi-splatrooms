using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

/*
 * GameManager
 * Handles top-level game state.
 */
public class GameManager : MonoBehaviour {

  public GameObject loadingOverlay;

  float _startingScore = 0;

  public void Start() {
    _startingScore = 0;
    loadingOverlay.GetComponent<LoadingOverlay>().FadeOut();
  }

  public void ShowEndGame() {
    loadingOverlay.GetComponent<LoadingOverlay>().FadeIn();
    StartCoroutine(ShowEndGameTransition());
  }

  IEnumerator ShowEndGameTransition() {
    yield return new WaitForSeconds(1.5f); // Wait for transition to finish
    SceneManager.LoadSceneAsync("EndGameScene");
  }

  // Takes a non-normalized percentage score value between 0 and 1
  public void SetCurrentScore(float score) {
    // The first score received is considered to be "0%" for display purposes
    if (_startingScore == 0) {
      _startingScore = score;
    }

    float normalizedScore = Mathf.InverseLerp(_startingScore, 1.0f, score);
    PlayerPrefs.SetFloat("PlayerScore", normalizedScore);
  }
}