using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public float speed = 1.0f;

	private Vector3 targetPosition;

	// Use this for initialization
	void Start () {
		targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		targetPosition.z -= speed;
		Vector3 t = targetPosition;
		float noise = targetPosition.z * 0.3f;
		t.x += Mathf.PerlinNoise(noise,0)*1.0f;
		t.y += Mathf.PerlinNoise(0,noise)*1.0f;

		transform.position = Vector3.MoveTowards(transform.position, t, speed*2.0f);

		if(transform.position.z+30 < GameManager.Instance.GetPlayerPosition().z) {
		
			Destroy(gameObject);
		}
	}
}
