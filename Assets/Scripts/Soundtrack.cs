using UnityEngine;
using System.Collections;

public class Soundtrack : MonoBehaviour {
	public float volume = 0.5f;
	// Use this for initialization
	void Start () {
		StartCoroutine("FadeIn");
	}

	IEnumerator FadeIn() {
		yield return new WaitForSeconds(1.0f);
	
		while(audio.volume < volume) {
			audio.volume += 0.01f;
			yield return new WaitForSeconds(0.02f);
		}
	}

	IEnumerator FadeOut() {
		yield return new WaitForSeconds(1.0f);
	}
	
	public void FadeOutNow() {
		StartCoroutine("FadeOut");
		            
	}
}
