using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Extensions;
using System;
using System.Linq;
using System.Collections;

public class GameManagerUI : MonoBehaviour {

	public GameManager Manager;
	public Image CastleIntegrityBar;
	public Text IdleHeroesText;
	public Text KingdomPopulation;
	public Text Gold;

	void Awake() {


		CastleIntegrityBar = transform.FindChildRecursive("CastleIntegrity_Bar").GetComponent<Image>();
		IdleHeroesText = transform.FindChildRecursive("IdleHeroes_Text").GetComponent<Text>();
		KingdomPopulation = transform.FindChildRecursive("KingdomPop_Text").GetComponent<Text>();
		Gold = transform.FindChildRecursive("Gold_Text").GetComponent<Text>();
	}

	internal void UpdateIdleHeroCount() {

		// While this is normally bad practice, there shouldnt be more than 10-15 heroes ever.
		Hero[] idleHeroes = Manager.Heroes.Values.Where(hero => hero.Idle).ToArray();
		Manager.IdleHeroCount = idleHeroes.Length;
		IdleHeroesText.text = "Idle Heroes: " + Manager.IdleHeroCount.ToString();
	}

	internal void UpdateGold(int gold) {

		Gold.text = "Gold: " + gold.ToString();
	}

	internal void UpdatePopulation(int population) {

		KingdomPopulation.text = "Kingdom Population: " + population.ToString();
	}

}
