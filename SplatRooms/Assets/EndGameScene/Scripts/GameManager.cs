using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EndGameScene {
	public class GameManager : MonoBehaviour {

		public GameObject guiObject;

		// Use this for initialization
		void Start () {
			StartCoroutine(AnimateInAfterDelay());
		}

		IEnumerator AnimateInAfterDelay() {
			yield return new WaitForSeconds(0.5f);
			guiObject.GetComponent<Animator>().SetBool("In", true);
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}