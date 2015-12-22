using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using Random = UnityEngine.Random;

public class KingdomAlert : MonoBehaviour {

	public Guid AlertID;
	public string AlertTitle;
	public string AlertDescription;
	public int Quantity;
	public float Difficulty;
	public string QuantityTextTemplate;
	public string EnemyName;
	public AlertTypeEnum AlertType;
	public Attributes UsedAttribute;

	private KingdomAlertUI _ui;

	private void Awake() {

		AlertID = Guid.NewGuid();
		_ui = GetComponent<KingdomAlertUI>();
	}

	private void Start() {

		GenerateNewAlert();		
	}

	private void GenerateNewAlert() {

		AlertTitle = "Orc Raiding Party";
		AlertDescription = "Orcs are raiding the lands of your kingdom!";
		int typeCount = Enum.GetNames(typeof(AlertTypeEnum)).Length;
		AlertType = (AlertTypeEnum)Random.Range(0, typeCount);

		int population = GameManager.Instance.KingdomPopulation;
		switch (AlertType) {
			case AlertTypeEnum.EnemySpotted:
				UsedAttribute = Attributes.Strength;
				// A weighted random quantity based on 1% of the current population.
				float wRand = Random.Range(0, population / 100f) + Random.Range(0, population / 100f);
				Quantity = (int)Mathf.Clamp(wRand, 1, Mathf.Infinity) + 1;
				QuantityTextTemplate = "{0} {1}s have been spotted near this area!";
				EnemyName = "Orc";
				Difficulty = GetDifficulty();
				break;
		}
	}

	private float GetDifficulty() {

		float low = 1f;
		float high = 0f;
		Hero hero;
		foreach (var kvPair in GameManager.Instance.Heroes) {
			hero = kvPair.Value;
			if (hero.Strength < low) {
				low = hero.Strength;
			}
			if (hero.Wisdom < low) {
				low = hero.Wisdom;
			}
			if (hero.Cunning < low) {
				low = hero.Cunning;
			}
			if (hero.Strength < high) {
				high = hero.Strength;
			}
			if (hero.Wisdom > high) {
				high = hero.Wisdom;
			}
			if (hero.Cunning > high) {
				high = hero.Cunning;
			}
		}
		return Random.Range(low, high);
	}

	public void DeselectUI() {

		_ui.Deselect();
	}

	public enum AlertTypeEnum {
		EnemySpotted,
	}
}
