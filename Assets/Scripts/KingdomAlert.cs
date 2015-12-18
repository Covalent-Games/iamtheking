﻿using UnityEngine;
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
				// A weighted random quantity based on 1% of the current population.
				float wRand = Random.Range(0, population / 100f) + Random.Range(0, population / 100f);
				Quantity = (int)Mathf.Clamp(wRand, 1, Mathf.Infinity) + 1;
				QuantityTextTemplate = "{0} {1}s have been spotted near this area!";
				EnemyName = "Orc";
				Difficulty = UnityEngine.Random.value;
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