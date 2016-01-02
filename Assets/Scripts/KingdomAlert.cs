using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.Extensions;

public class KingdomAlert : MonoBehaviour {

	public Guid AlertID;
	public string AlertTitle;
	public string AlertDescription;
	public int Quantity;
	public float Difficulty;
	public string QuantityTextTemplate;
	public string EnemyName;
	public AlertTypeEnum AlertType = AlertTypeEnum.None;
	public Attributes UsedAttribute;
	public TimeSpan Duration;
	public bool Blocked = false;
	public int HeroesAtAlert = 0;

	private Text _timer;
	private KingdomAlertUI _ui;
	private Sprite _goodAlertSprite;

	private void Awake() {

		AlertID = Guid.NewGuid();
		_ui = GetComponent<KingdomAlertUI>();
		_timer = transform.FindChildRecursive("Timer_Text").GetComponent<Text>();
	}

	private void Start() {

		SetRandomAlertType();
		SetValuesByAlertType();
	}

	private void SetRandomAlertType() {

		int typeCount = Enum.GetNames(typeof(AlertTypeEnum)).Length;
		AlertType = (AlertTypeEnum)Random.Range(1, typeCount);
	}

	/// <summary>
	/// Handles all the random and finer details of the alert based on the AlertTypeEnum.
	/// If AlertTypeEnum isn't set this will raise an exception.
	/// </summary>
	public void SetValuesByAlertType() {

		if (AlertType == AlertTypeEnum.None) {
			throw new Exception("The alert does not have a set AlertType!");
		}

		switch (AlertType) {
			case AlertTypeEnum.EnemySpotted:
				SetUpEnemySpottedAlert(GameManager.Instance.KingdomPopulation);
				break;
			case AlertTypeEnum.NewCitySite:
				SetUpNewCitySite();
				break;
		}
	}

	private void SetUpNewCitySite() {

		_timer.transform.parent.gameObject.SetActive(false);
		GetComponent<SpriteRenderer>().color = new Color(0f, 160/255f, 1f);
		AlertTitle = "Possible New City Site!";
		AlertDescription = "Kimgdom Excavators have located what they believe to be prime lands" +
			" for a new city. The King should choose the best to found the kingdom's newest city!";
		UsedAttribute = Attributes.Wisdom;
		// Number of sites in the area.
		Quantity = Random.Range(1, 4);
		QuantityTextTemplate = "{0} {1} been discovered and await the King's choice before breaking ground.";
	}

	private void SetUpEnemySpottedAlert(int population) {

		EnemyName = NameGenerator.NewEnemyName();
		Difficulty = GetDifficulty();
		AlertTitle = EnemyName + " Raiding Party";
		AlertDescription = EnemyName + "s are raiding the lands of your kingdom!";
		UsedAttribute = Attributes.Strength;
		// A weighted random quantity based on 1% of the current population.
		float wRand = Random.Range(0, population / 100f) + Random.Range(0, population / 100f);
		Quantity = (int)Mathf.Clamp(wRand, 1, Mathf.Infinity) + 1;
		QuantityTextTemplate = "{0}{1}s have been spotted near this area!";
		Duration = GetTimerDuration();
		StartCoroutine(UpdateTimerRoutine());
		_timer.transform.parent.gameObject.SetActive(true);
	}

	private TimeSpan GetTimerDuration(int minSeconds=60, int maxSeconds=300) {

		return TimeSpan.FromSeconds(Random.Range(minSeconds, maxSeconds));
	}

	/// <summary>
	/// Returns a randomly generated Alert Difficulty based on the skills of the existing heroes.
	/// </summary>
	/// <returns></returns>
	private float GetDifficulty() {

		float low = 1f;
		float high = 0f;
		Hero hero;
		// Get the lowest and highest of any stat for the global difficulty range.
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
			if (hero.Strength > high) {
				high = hero.Strength;
			}
			if (hero.Wisdom > high) {
				high = hero.Wisdom;
			}
			if (hero.Cunning > high) {
				high = hero.Cunning;
			}
		}
		// Scale the high end of the difficulty by 50%.
		return Random.Range(low, high * 1.5f);
	}

	public void DeselectUI() {

		_ui.Deselect();
	}

	public enum AlertTypeEnum {

		None,
		EnemySpotted,
		NewCitySite,
	}

	private IEnumerator UpdateTimerRoutine() {

		TimeSpan subtractionTime;
		while (enabled) {
			if (HeroesAtAlert == 0) {
				subtractionTime = new TimeSpan(0, 0, 0, 0, (int)(Time.deltaTime * 5 * 1000));
				Duration = Duration.Subtract(subtractionTime);
				_timer.text = string.Format("{0:D2}:{1:D2}", Duration.Minutes, Duration.Seconds);
				if (Duration.TotalSeconds <= 0) {
					DoBadThing();
					Duration = GetTimerDuration();
				}
			}
			// Delay to reduce GC allocations. They were getting a little high. About 433Kb/frame/alert.
			yield return new WaitForSeconds(Time.deltaTime * 5);
		}
	}

	private void DoBadThing() {
		
		switch (AlertType) {
			case AlertTypeEnum.EnemySpotted:
				DoEnemySpottedFailure();
				break;
		}
	}

	private void DoEnemySpottedFailure() {

		// On easier difficulties the alert will be close by. Harder it will be farther away.
		//TODO: Keep this on the map.
		Vector3 newPos = new Vector2(Random.Range(-4f, 4f), Random.Range(-4f, 4f)) * Mathf.Max(1f, Difficulty);
		newPos += transform.position;
		Transform closestSettlement = Map.KingdomCastle.transform;
		float shortestDistance = Vector3.Distance(newPos, closestSettlement.position);
		foreach (var kvPair in GameManager.Instance.Villages) {
			float currentDistance = Vector3.Distance(newPos, kvPair.Value.transform.position);
			if (currentDistance < shortestDistance) {
				closestSettlement = kvPair.Value.transform;
				shortestDistance = currentDistance;
			}
		}
		if (closestSettlement == Map.KingdomCastle.transform) {
			Map.KingdomCastle.Integrity -= Difficulty / 2f;
		}
		Vector3 vectorToNearestSettlement = closestSettlement.position - newPos;
		vectorToNearestSettlement.Normalize();
		// Move the new position towards the closest village/castle.
		newPos += (vectorToNearestSettlement * 2.5f);
		// Move away from existing alert if applicable.
		//Bounds bounds = new Bounds(newPos, Vector3.zero);
		//foreach (var kvPair in GameManager.Instance.Alerts) {
		//	if (Vector3.Distance(kvPair.Value.transform.position, newPos) <= 2f) {
		//		Vector3 direction = newPos - kvPair.Value.transform.position;
		//		direction.Normalize();
		//		newPos += direction * 1f;
		//		bounds.Encapsulate(kvPair.Value.transform.position);
		//	}
		//}
		//// If the bounds size is 0 then we are nowhere near another alert and we don't need to modify newPos.
		//if (bounds.size != Vector3.zero) {
		//	newPos = bounds.center;
		//}

		KingdomAlert newAlert = GameManager.Instance.InstantiateNewAlert(newPos);

		newAlert.AlertType = AlertTypeEnum.EnemySpotted;
		newAlert.SetValuesByAlertType();
		// Set the difficulty of the new alert to this "parent" difficulty.
		newAlert.Difficulty = Difficulty;
		
	}
}
