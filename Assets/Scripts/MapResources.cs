using UnityEngine;

public class MapResources {

	public GameObject CastlePrefab;
	public GameObject HeroPrefab;
	public GameObject EventAlertPrefab;
	public GameObject GenericAlertPrefab;
	public GameObject VillagePrefab;


	public MapResources() {

		LoadInitialAssets();
	}

	public void LoadInitialAssets() {

		CastlePrefab = (GameObject)Resources.Load("MapObjects/Castle_prefab");
		HeroPrefab = (GameObject)Resources.Load("MapObjects/Hero_prefab");
		EventAlertPrefab = (GameObject)Resources.Load("MapObjects/Alert_prefab");
		GenericAlertPrefab = (GameObject)Resources.Load("MapObjects/GenericAlert_prefab");
		VillagePrefab = (GameObject)Resources.Load("MapObject/Village_prefab");
	}
}