using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {


	public GUIText textPoints;
	public AudioClip soundSting;
	public AudioClip soundCrash;
	public AudioClip soundLaunch;
	public Transform particleSystem;
	public Transform particlesThrust;
	private ParticleSystem ps;
	public Color thrustColorOutside;
	public Color thrustColorInside;
	public Transform ship;
	public Transform shipParticles;
	public Color startColor;
	public Color endColor;
	private Color currentColor;
	private Color targetColor;
	private bool toggleColor;
	public GameObject areaLight;
	public Color[] wallColorPalette;

	public float moveSpeed = 0.1f;
	public float currentSpeed;
	public float rotationSpeed = 0.1f;
	private float currentRotationSpeed;
	private int direction = 1;

	private Vector3 currentRotation = Vector3.zero;
	private Vector3 targetRotation = Vector3.zero;
	private bool isRotating = false;

	private float shipEnergy = 100.0f;

	public float waterEnergySuckAmount = 0.1f;
	public float waterEnergySuckTime = 0.5f;


	public Texture2D energyBarBg;
	public Texture2D energyBarFill;
	
	private AudioSource audio;

	private bool isDead = false;
	private Mesh explodeMesh;

	private float points = 0;
	// Use this for initialization
	void Start () {

		audio = GetComponent<AudioSource>();
		ps = particlesThrust.GetComponent<ParticleSystem>();

		explodeMesh = new Mesh();
		explodeMesh = ship.GetComponent<MeshFilter>().mesh;

		StartLevel();

		StartCoroutine("zoomIn");
		StartCoroutine("RotateShip");
		StartCoroutine("SwitchColors");
		StartCoroutine("InsideWater");
		StartCoroutine("OutsideWater");
		StartCoroutine("SpeedUp");
	}

	public bool IsInsideWater() {
		if(currentRotation.z > 270 || currentRotation.z < 90) return true;
		return false;
	}

	void StartLevel() {
		currentSpeed = moveSpeed;
		currentRotationSpeed = rotationSpeed;
		points = 0;

		isDead = false;
		toggleColor = true;
		audio.PlayOneShot(soundLaunch);

		currentColor = wallColorPalette[Random.Range(0,wallColorPalette.Length)];;
		targetColor = currentColor;
		areaLight.GetComponent<Light>().color =  currentColor;


		shipEnergy = 100.0f;
		textPoints.text = "0";
	}

	IEnumerator SpeedUp() {
		while(true) {
			yield return new WaitForSeconds(25.0f);
			currentSpeed += 0.05f;
			currentRotationSpeed += 0.5f;
		}
	}

	IEnumerator InsideWater() {

		while(true) {

			if(!isDead && IsInsideWater()) {
				shipEnergy -= waterEnergySuckAmount;
				if(shipEnergy < 0.0f) Die();
				shipEnergy = Mathf.Clamp(shipEnergy, 0, 100.0f);
				yield return  new WaitForSeconds(waterEnergySuckTime);
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator OutsideWater() {
		while(true) {

			if(!isDead && !IsInsideWater()) {
				shipEnergy += waterEnergySuckAmount;
				shipEnergy = Mathf.Clamp(shipEnergy, 0, 100.0f);
				yield return  new WaitForSeconds(waterEnergySuckTime*2);
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator zoomIn() {
		Camera.main.fieldOfView = 100;
		while(true) {
			Camera.main.fieldOfView -= 1.0f;
			if(Camera.main.fieldOfView <= 60.0f) {
				Camera.main.fieldOfView = 60.0f;
				break;
			}
			yield return new WaitForSeconds(0.01f);
		}
	}

	IEnumerator RotateShip() {
		while(true) {

			currentRotation.z += direction * currentRotationSpeed;
			if(currentRotation.z > 360.0f) currentRotation.z -= 360.0f;

			if(currentRotation.z < 0) currentRotation.z += 360.0f;
			transform.eulerAngles = currentRotation;
			areaLight.GetComponent<Light>().color = currentColor;
			yield return new WaitForEndOfFrame();
		}

	}

	IEnumerator SwitchColors() {
		while(true) {
			yield return new WaitForSeconds(2.0f);
			startColor = wallColorPalette[Random.Range (0,wallColorPalette.Length)];
			yield return new WaitForSeconds(2.0f);
			endColor = wallColorPalette[Random.Range (0,wallColorPalette.Length)];
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(isDead) return;

		if(Input.GetAxis("Horizontal")> 0) {
			direction = 1;
			toggleColor = !toggleColor;
		}

		if(Input.GetAxis("Horizontal")< 0 ) {
			direction = -1;
			toggleColor = !toggleColor;
		}

		// colors
		currentColor = Color.Lerp(currentColor, (toggleColor) ? endColor : startColor, Time.deltaTime);

	
		// thrust
		if(IsInsideWater()) {
			ps.renderer.material.SetColor("_TintColor", thrustColorInside);
		} else {
			ps.renderer.material.SetColor("_TintColor", thrustColorOutside);
		}

		// move
		transform.Translate(new Vector3(0,0,currentSpeed));
		points += 0.1f;
		textPoints.text = Mathf.Floor (points).ToString();

		particleSystem.position = transform.position;
	}

	IEnumerator ExplodeMesh() {

		shipParticles.gameObject.SetActive(false);

		ship.GetComponent<MeshFilter>().mesh = explodeMesh;
		float time = 0;
		while(time <= 2.0f) {

			Vector3[] verts = explodeMesh.vertices;
			for(int i=0;i<verts.Length;i++) {
				Vector3 v = verts[i];
				Vector3 n = explodeMesh.normals[i];
				n.Normalize();
				v += n*0.02f;
				verts[i] = v;
			}

			explodeMesh.vertices = verts;

			yield return new WaitForSeconds(0.01f);
			time += Time.deltaTime;
		}

		Application.LoadLevel(0);
	}

	public void TakeEnergy(float amount) {
		audio.PlayOneShot(soundSting);
		shipEnergy -= amount;
	}

	public void Die() {
		if(isDead) return;
		shipEnergy = 0.0f;
		isDead = true;
		audio.PlayOneShot(soundCrash);
		StartCoroutine("ExplodeMesh");
	}

	void OnGUI() {



		float startX = (Screen.width - 300.0f) /2.0f;
		GUI.DrawTexture(new Rect(startX,10,300,40), energyBarBg);

		float fillWidth = (shipEnergy / 100.0f) * 261.0f;
		GUI.DrawTexture(new Rect(startX+19,13,fillWidth,34), energyBarFill);
	}
}
