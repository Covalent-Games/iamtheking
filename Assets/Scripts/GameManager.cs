using UnityEngine;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;
	public static float HeroSpeed = 2f;

	public Map KingdomMap;
	public Dictionary<Guid, Hero> Heroes = new Dictionary<Guid, Hero>();
	public Dictionary<Guid, KingdomAlert> Alerts = new Dictionary<Guid, KingdomAlert>();
	public Dictionary<Guid, Village> Villages = new Dictionary<Guid, Village>();
	public int IdleHeroCount = 0;
	public int KingdomPopulation = 100;
	public int Gold = 0;
	public QuestBuilder QuestCreator;
	public CastleUI KingdomCastleUI;

	private GameManagerUI _ui;

	private void Awake() {

		//Ensure this is the only GameManager.
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}

		KingdomMap = FindObjectOfType<Map>();
		_ui = GetComponent<GameManagerUI>();
		_ui.Manager = this;
		QuestCreator = GetComponent<QuestBuilder>();
		KingdomCastleUI = GetComponent<CastleUI>();
	}

	private void Start () {

		Hero hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero.Allegiance = 1f;

		StartCoroutine(SpawnAlertsRoutine());

	}

	private IEnumerator SpawnAlertsRoutine() {

		Vector2 spawnLoc;
		while (gameObject.activeSelf) {
			yield return new WaitForSeconds(Random.Range(1f, 5f));
			if (Alerts.Count < 2) {
				spawnLoc = new Vector2(Random.Range(-12f, 12f), Random.Range(-12f, 12f));
				InstantiateNewAlert(spawnLoc);
				Debug.Log("NEW ALERT @ " + spawnLoc); 
			}
		}
	}

	private KingdomAlert InstantiateNewAlert(Vector2 vector2) {

		//TODO: Generate some kind of indicator that a new alert has spawned.
		GameObject newAlert = 
			(GameObject)Instantiate(Map.Assets.EventAlertPrefab, vector2, Quaternion.identity);

		KingdomAlert alert = newAlert.GetComponent<KingdomAlert>();
		Alerts.Add(alert.AlertID, alert);
		newAlert.SetActive(true);

		return alert;
	}

	public void RemoveAlert(Guid id) {

		KingdomAlert alert = Alerts[id];
		alert.gameObject.SetActive(false);
		Alerts.Remove(id);
		Destroy(alert);
	}

	private void Update () {

		// TODO If there's bad performance, make this event based.
		_ui.UpdateIdleHeroCount(IdleHeroCount);
		_ui.UpdateGold(Gold);
		_ui.UpdatePopulation(KingdomPopulation);

	}

	public void UpdateCastleIntegrityUI(int current, int max) {

		_ui.CastleIntegrityBar.fillAmount = (float)current / (float)max;
	}

	/// <summary>
	/// Create a new hero at a given position.
	/// </summary>
	/// <param name="position">Where to spawn the hero.</param>
	/// <param name="active">Whether or not the hero GameObject should be active to start.</param>
	/// <returns>Hero instance</returns>
	public Hero InstantiateNewHero(Vector3 position, bool active=false) {

		GameObject heroGO = Instantiate(Map.Assets.HeroPrefab);
		Hero hero = heroGO.GetComponent<Hero>();
		heroGO.transform.position = position;
		Heroes.Add(hero.ID, hero);
		IdleHeroCount++;
		_ui.IdleHeroesText.text = "Idle Heroes: " + IdleHeroCount;
		heroGO.gameObject.SetActive(active);

		return hero;
	}

	internal static void GameOver() {

		throw new NotImplementedException();
	}
}
