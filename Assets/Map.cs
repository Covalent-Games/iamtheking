using UnityEngine;
using System.Collections;
using System;

public class Map : MonoBehaviour {

	public static GameObject Castle;

	public static MapResources Assets;


	void Awake() {

		Assets = new MapResources();
	}

	void Start() {

		RandomMapROtate();
		PlaceKingdom();

	}

	private void PlaceKingdom() {

		Castle = Instantiate(Assets.CastlePrefab);
		Castle.transform.position = 
			new Vector2(UnityEngine.Random.Range(-12f, 12f), 
						UnityEngine.Random.Range(-12f, 12f));
		Camera.main.transform.position = 
			new Vector3(Castle.transform.position.x, 
						Castle.transform.position.y, 
						Camera.main.transform.position.z);
	}

	private void RandomMapROtate() {

		float rotate = UnityEngine.Random.Range(1, 3) * 90f;
		transform.Rotate(0f, 0f, rotate);
	}

	void Update () {
	
	}
}
