using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class Hero : MonoBehaviour {

	public Guid ID;
	public bool Idle = true;
	public string Name;
	public ClassTypeEnum ClassType = ClassTypeEnum.Warrior;
	public List<QuestObject> Quests = new List<QuestObject>();
	public Sprite UIImage;
	public float Allegiance = 0.5f;
	public int MaxHealth = 10;
	public float Strength = 0.1f;
	public float Wisdom = 0.1f;
	public float Cunning = 0.1f;
	internal int Level = 1;
	internal int Health	{
		get	{
			return _health;
		}
		set	{
			_health = value;
			if (_health <= 0) {
				Die();
			} else if (_health > MaxHealth) {
				_health = MaxHealth;
			}
		}
	}
	internal float Exp{
		get	{
			return _exp;
		}
		set	{
			_exp = value;
			if (_exp >= ExpToLevel) {
				LevelUp();
			}
		}
	}

	internal float ExpToLevel = 2f;

	private float _exp = 0f;
	private int _health = 10;

	public enum ClassTypeEnum {
		Warrior,
		Mage,
		Rogue,
	}

	void Awake() {

		ID = Guid.NewGuid();
		ClassType = (ClassTypeEnum)Random.Range(0, 3);

		Name = NameGenerator.New();
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

		switch (ClassType) {
			case ClassTypeEnum.Warrior:
				Strength = Random.Range(.1f, .2f);
				Wisdom = Random.Range(.02f, .08f);
				Cunning = Random.Range(.03f, .1f);
				MaxHealth = 10;
				break;
			case ClassTypeEnum.Mage:
				Wisdom = Random.Range(.1f, .2f);
				Strength = Random.Range(.02f, .08f);
				Cunning = Random.Range(.03f, .1f);
				spriteRenderer.sprite = Resources.Load<Sprite>("RawImages/icon_mage_wand_01");
				MaxHealth = 8;
				break;
			case ClassTypeEnum.Rogue:
				Cunning = Random.Range(.1f, .2f);
				Wisdom = Random.Range(.02f, .08f);
				Strength = Random.Range(.03f, .1f);
				spriteRenderer.sprite = Resources.Load<Sprite>("RawImages/icon_rogue_daggers_01");
				MaxHealth = 9;
				break;
		}
		Health = MaxHealth;
		UIImage = spriteRenderer.sprite;
	}

	public void Die() {

		GameManager.Instance.RemoveHero(ID);
	}

	private void LevelUp() {

		// These are all placeholder numbers.
		switch (ClassType) {
			case ClassTypeEnum.Warrior:
				Strength += Random.Range(.09f, .15f);
				Wisdom += Random.Range(.02f, .08f);
				Cunning += Random.Range(.03f, .1f);
				MaxHealth += 3;
				break;
			case ClassTypeEnum.Mage:
				Wisdom += Random.Range(.09f, .15f);
				Strength += Random.Range(.02f, .08f);
				Cunning += Random.Range(.03f, .1f);
				MaxHealth += 1;
				break;
			case ClassTypeEnum.Rogue:
				Cunning += Random.Range(.09f, .15f);
				Wisdom += Random.Range(.02f, .08f);
				Strength += Random.Range(.03f, .1f);
				MaxHealth += 2;
				break;
		}
		Health = MaxHealth;
		ExpToLevel += ExpToLevel;
		Level++;
		_exp = 0f;
		NotificationManager.DisplayNotification(string.Format(
			"{0} is now level {1}!", Name, Level), Color.yellow);
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
						// If hero finishes personal objective or the entire alert is cleared.
						while (currentQuest.Quantity > 0 && currentQuest.Alert.Quantity > 0) {
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

	internal void PayHero(QuestObject quest, int recommendedReward) {

		if (Allegiance > 0.95f) {
			if (Random.value < 0.25f) {
				Debug.Log("The hero has decided to run this one for free.");
				return;
			}
		}

		float reward = quest.GoldReward;
		float percentage = reward / (recommendedReward * quest.Quantity);
		if (percentage > 1f) {
			Allegiance += percentage * 0.05f;
		} else if (percentage < 1f) {
			Allegiance -= (0.999f - percentage) * 0.2f;
		}
		GameManager.Instance.Gold -= quest.GoldReward;
	}

	private void AttackEnemy(QuestObject currentQuest) {

		float winChance = Mathf.Clamp(Strength / currentQuest.Difficulty, .01f, .90f);
		float roll = UnityEngine.Random.value;
		if (roll <= winChance) {
			Debug.Log(string.Format("{0} rolled a {1} against {2} and has slain a foe!",
				Name, roll, winChance));
			// Update the currentQuest tracker, and also remove the enemy from the alert.
			currentQuest.Quantity--;
			currentQuest.Alert.Quantity--;
			Exp++;
		} else {
			Debug.Log(Name + " has taken damage!");
			Health--;
			// 1/2 percent allegiance loss per damage taken. 
			//TODO: This needs to scale down for higher health heroes.
			Allegiance -= 0.005f;
		}
	}
}
