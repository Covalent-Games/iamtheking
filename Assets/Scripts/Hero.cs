using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Hero : MonoBehaviour {

	public Guid ID;
	public bool Idle = true;
	public string Name;
	public ClassTypeEnum ClassType = ClassTypeEnum.Warrior;
	public List<QuestObject> Quests = new List<QuestObject>();
	public Sprite UIImage;
	public float Allegiance = 0.5f;
	public int Health = 10;
	public int MaxHealth = 10;
	public float Strength = 0.1f;
	public float Wisdom = 0.1f;
	public float Cunning = 0.1f;
	internal int Level = 1;
	internal float Exp = 0f;
	internal float ExpToLevel = 2f;

	public enum ClassTypeEnum {
		Warrior,
		Mage,
		Rogue,
	}

	void Awake () {

		ID = Guid.NewGuid();
		UIImage = GetComponent<SpriteRenderer>().sprite;
		//TODO: Name generator;
		string[] names = new string[5] {
			"Hacker",
			"Smacker",
			"Wacker",
			"Cracker",
			"Bob",
		};
		Name = names[UnityEngine.Random.Range(0, 4)];
		Strength = UnityEngine.Random.value;
		Wisdom = UnityEngine.Random.value;
		Cunning = UnityEngine.Random.value;
		Exp = UnityEngine.Random.value + UnityEngine.Random.value;
		Allegiance = UnityEngine.Random.value;
	}

	public void BeginQuest() {

		StartCoroutine(ExecuteQuestRoutine());
	}

	private IEnumerator ExecuteQuestRoutine() {

		Idle = false;
		QuestObject currentQuest;

		while (Quests.Count > 0) {
			currentQuest = Quests[0];
			Debug.Log(Name + " taking care of the " + currentQuest.Alert.AlertTitle);
			// Move towards our current quest objective.
			while (transform.position != currentQuest.Destination) {
				transform.position = 
					Vector3.MoveTowards(
						transform.position,
						currentQuest.Destination, 
						Time.deltaTime * GameManager.HeroSpeed);
				yield return null;
			} 
			// Work towards completing the objective.
			if (currentQuest.Alert != null) {
				switch (currentQuest.Goal) {
					case QuestObject.QuestType.Slayer:
						while (currentQuest.Quantity > 0) {
							yield return new WaitForSeconds(2f);
							AttackEnemy(currentQuest);
						} 
						break;
				}
				if (currentQuest.Alert.Quantity <= 0) {
					GameManager.Instance.RemoveAlert(currentQuest.Alert.AlertID); 
				}
			}
			// We've accomplished the hero specific objective so remove the QuestObject.
			// The alert/event may or may not be completed.
			Quests.Remove(currentQuest);
			yield return null;
		} 
		
		//Return to the castle because all objectives are complete.
		while (transform.position != Map.KingdomCastle.transform.position) {
			transform.position =
				Vector2.MoveTowards(
					transform.position,
					Map.KingdomCastle.transform.position,
					Time.deltaTime * GameManager.HeroSpeed);
			yield return null;
		}
		Idle = true;
		gameObject.SetActive(false);
	}

	private void AttackEnemy(QuestObject currentQuest) {

		float winChance = Mathf.Clamp(Strength / currentQuest.Difficulty, .01f, .90f);
		float roll = UnityEngine.Random.value;
		if (roll <= winChance) {
			Debug.Log(string.Format("{0} rolled a {1} against {2} and has slayn a foe!",
				Name, roll, winChance));
			// Update the currentQuest tracker, and also remove the enemy from the alert.
			currentQuest.Quantity--;
			currentQuest.Alert.Quantity--;
			Exp++;
		} else {
			Debug.Log(Name + " has taken damage!");
			Health--;
		}
	}
}
