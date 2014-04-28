using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public Vector3 rotationStart;
	public Vector3 rotationEnd;

	// Use this for initialization
	void Start () {
		camera.fieldOfView = 180.0f;
		StartCoroutine("ZoomIn");

	}

	IEnumerator ZoomIn() {
		while(camera.fieldOfView > 45.0f) {
			camera.fieldOfView -= 2.0f;
			yield return new WaitForSeconds(0.01f);
		}

	
	}

	void Update() {
		if(Input.GetKeyDown(KeyCode.Space)) {
			Application.LoadLevel ("Scene");
		}

		if(Input.GetKeyDown(KeyCode.R)) {
			Application.CaptureScreenshot("main.png");

		}
	}
}
