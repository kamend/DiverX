using UnityEngine;
using System.Collections;

public class ShipController : MonoBehaviour {

	private float rotation = 30.0f;
	private float speed = 1.5f;
	// Use this for initialization
	void Start () {
		StartCoroutine("SlightRotation");
	}

	IEnumerator SlightRotation() {

		while(true) {
			for(float r = -rotation;r <= rotation; r += speed) {
				Vector3 rot = new Vector3(0,0,r);
				rot.z += transform.parent.eulerAngles.z;
				transform.eulerAngles = rot;
		
				yield return new WaitForSeconds(0.02f);
			}

			for(float r = rotation;r >= -rotation; r-=speed) {
				Vector3 rot = new Vector3(0,0,r);
				rot.z += transform.parent.eulerAngles.z;
				transform.eulerAngles = rot;
				yield return new WaitForSeconds(0.02f);
			}
		}
	}
	void Die() {
		transform.parent.GetComponent<PlayerController>().Die();
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag == "Obstacle") {
			Die();
		}

		if(other.tag == "Enemy") {
			transform.parent.GetComponent<PlayerController>().TakeEnergy(10);

		}
	}
}
