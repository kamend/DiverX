using UnityEngine;
using System.Collections;

public class DisableChilds: MonoBehaviour {

	void OnEnable() {
		gameObject.SetActive(false);
	}
}
