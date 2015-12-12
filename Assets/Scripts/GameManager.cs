using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public Map KingdomMap;
	public Dictionary<Guid, Hero> Heroes = new Dictionary<Guid, Hero>();
	public List<Village> Villages = new List<Village>();
	public int IdleHeroCount = 0;
	public int KingdomPopulation = 100;
	public int Gold = 0;

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
	}

	private void Start () {

		Hero hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero = InstantiateNewHero(Map.KingdomCastle.transform.position);
		hero.Allegiance = 1f;

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
