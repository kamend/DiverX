using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager Instance = null;

	public Transform player;
	public GameObject prefabTunnel;
	public GameObject prefabObstacle;
	public GameObject prefabEnemy;


	public Queue<GameObject> tunnels;

	private int numTunnelsAtStart = 30;
	private int tunnelsDistance = 10;
	private int nextTunnelIdx;

	private int screenshotIndex = 0;
	private int lastTunnelWithObstacleIdx = -1;

	void OnEnable() {
		if(Instance == null) {
			Instance = this;
		}
	}


	// Use this for initialization
	void Start () {
		InitLevel();
		StartCoroutine("BuildTunnels");
	}

	void InitLevel() {
		nextTunnelIdx = -2;
		tunnels = new Queue<GameObject>();

		// draw initial tunnels

		Vector3 startPosition = new Vector3(0,0,-20);
		Vector3 nextPosition = startPosition;
		for(int i=0;i<numTunnelsAtStart;i++) {
			AddTunnel(nextPosition);
			nextPosition.z += tunnelsDistance;
		}


	}

	public Vector3 GetPlayerPosition() {
		return player.transform.position;
	}

	IEnumerator BuildTunnels() {
		int currentIdx = getCurrentTunnelIndex();
		while(true) {
			int idx = getCurrentTunnelIndex();
			if(idx != currentIdx) {
		
					Vector3 position = new Vector3(0,0,nextTunnelIdx * tunnelsDistance);
					AddTunnel(position);
					RemoveLastTunnel();
			
			}
			currentIdx = idx;
			yield return new WaitForSeconds(0.01f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.R)) {
			Application.CaptureScreenshot("screen"+screenshotIndex+".png");
			screenshotIndex++;
		}
	}

	void AddTunnel(Vector3 pos) {

		GameObject go = GameObject.Instantiate(prefabTunnel, pos, Quaternion.identity) as GameObject;
		tunnels.Enqueue(go);
	
		// add obstacles
		int obstacleChance = Random.Range(0,50);
		if(obstacleChance > 35 && nextTunnelIdx > 4) {


			if(lastTunnelWithObstacleIdx > 0 && (Mathf.Abs (lastTunnelWithObstacleIdx-nextTunnelIdx)) < 2) {}
			else {

				float obstaclePos = Mathf.Floor(Random.Range (5,7));
				obstaclePos = pos.z-obstaclePos;

				GameObject ob = GameObject.Instantiate(prefabObstacle, new Vector3(0,0,0), Quaternion.identity) as GameObject;
				ob.transform.parent = go.transform;
				ob.transform.position = new Vector3(0,0,obstaclePos);
				int rotationChance = Random.Range (0,100);
				float rotation = 0;
				if(rotationChance > 50) {
					rotation = 180;
				}
				ob.transform.eulerAngles = new Vector3(0,0,Random.Range (0,360));

				lastTunnelWithObstacleIdx = nextTunnelIdx;
			}

			// add enemy
			int numEnemies = Random.Range (0,2);
			for(int i=0;i<numEnemies;++i) {
				GameObject e = GameObject.Instantiate(prefabEnemy, Vector3.zero, Quaternion.identity) as GameObject;
				Vector3 ePos = new Vector3(0,0,0);
				ePos.z = pos.z - Random.Range (0,10);
				ePos.x = Random.Range (-0.5f,0.5f);
				ePos.y = Random.Range (-0.5f,0.5f);

				e.transform.position = ePos;

			}
		}

		nextTunnelIdx++;
	}

	void RemoveLastTunnel() {
		GameObject g = tunnels.Dequeue();
		Destroy (g);
	}

	int getCurrentTunnelIndex() {
		int idx = (int)Mathf.Floor (player.transform.position.z / (float) tunnelsDistance);

		return idx;
	}
}
