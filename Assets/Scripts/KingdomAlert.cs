using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

public class KingdomAlert : MonoBehaviour {

	public Guid AlertID;
	public string AlertTitle;
	public int Quantity;
	public string EnemyName;
	public AlertTypeEnum AlertType;

	private KingdomAlertUI _ui;

	private void Awake() {

		AlertID = Guid.NewGuid();
		_ui = GetComponent<KingdomAlertUI>();
	}

	private void Start() {

		GenerateNewAlert();		
	}

	private void GenerateNewAlert() {

		AlertTitle = "Orcs are raiding the lands!";
		int typeCount = Enum.GetNames(typeof(AlertTypeEnum)).Length;
		AlertType = (AlertTypeEnum)Random.Range(0, typeCount);

		int population = GameManager.Instance.KingdomPopulation;
		switch (AlertType) {
			case AlertTypeEnum.EnemySpotted:
				// A weighted quantity based on 1% of the current population.
				Quantity = (int)Mathf.Clamp(
					Random.Range(0, population / 100f) + Random.Range(0, population / 100f),
					1, 
					Mathf.Infinity) + 1;
				EnemyName = "Orcs";
				break;
		}
	}

	public void DeselectUI() {

		_ui.Deselect();
	}

	public enum AlertTypeEnum {
		EnemySpotted,
	}
}
