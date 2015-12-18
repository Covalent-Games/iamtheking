using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestObject {

	public Vector3 Destination;
	public QuestType Goal;
	public int Quantity;
	public float Difficulty;
	public int GoldReward;
	public KingdomAlert Alert;
	public Hero QuestingHero;

	public enum QuestType {
		Slayer,
		Founding,
	}

	public QuestObject(Hero hero, KingdomAlert alert) {

		QuestingHero = hero;
		Alert = alert;
		Difficulty = alert.Difficulty;
	}
}
