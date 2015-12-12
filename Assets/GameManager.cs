using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	public Map KingdomMap;
	public List<Hero> Heroes = new List<Hero>();
	public int IdleHeroCount = 0;

	private GameManagerUI _ui;

	void Awake() {

		//Ensure this is the only GameManager.
		if (Instance == null) {
			Instance = this;
		} else {
			Destroy(gameObject);
		}

		KingdomMap = FindObjectOfType<Map>();
		_ui = GetComponent<GameManagerUI>();
	}

	void Start () {

		InstantiateNewHero(Map.Castle.transform.position);

	}

	void Update () {

		UpdateIdleHeroCount();

	}

	private void UpdateIdleHeroCount() {

		// While this is normally bad practice, there shouldn't be more than 10-15 heroes ever.
		Hero[] idleHeroes = Heroes.Where(hero => hero.Idle).ToArray();
		if (idleHeroes.Length != IdleHeroCount) {
			IdleHeroCount = idleHeroes.Length;
			_ui.IdleHeroesText.text = "Idle Heroes: " + IdleHeroCount;
		}
	}

	private void InstantiateNewHero(Vector3 position) {

		GameObject hero = Instantiate(Map.Assets.HeroPrefab);
		hero.transform.position = position;
		Heroes.Add(hero.GetComponent<Hero>());
		IdleHeroCount++;
		_ui.IdleHeroesText.text = "Idle Heroes: " + IdleHeroCount;
		hero.gameObject.SetActive(false);

	}

	internal static void GameOver() {

		throw new NotImplementedException();
	}

	public void ButtonMcButton() {

		Debug.Log("Pressing button");
	}
}
