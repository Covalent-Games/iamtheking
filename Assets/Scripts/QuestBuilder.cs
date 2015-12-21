using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Extensions;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


public class QuestBuilder: MonoBehaviour {

	public static bool SelectingAlertForQuest = false;
	public static bool SelectingHeroForQuest = false;
	public static Guid HeroForQuest;
	public static Guid AlertForQuest;

	private QuestObject _questObject;

	// UI Elements
	private GameObject _questBuilderUI;
	private Text _questTitle;
	private Text _questDesc;
	// EnemySpotted UI
	private Transform _quantityAdjustContainer;
	private Text _killQuantity;
	private Text _killQuantityText;
	private Slider _killQuantitySlider;
	// Gold Reward UI
	private Text _goldRewardTotal;
	private Text _goldPerAction;
	private Text _notEnoughGold;
	private Button _decreaseGoldReward;
	private Button _increaseGoldReward;
	

	private void Awake() {

		_questBuilderUI = transform.FindChildRecursive("QuestBuilder_Panel").gameObject;
		_questTitle = transform.FindChildRecursive("QuestTitle_Text").GetComponent<Text>();
		_questDesc = transform.FindChildRecursive("QuestDesc_Text").GetComponent<Text>();
		_quantityAdjustContainer = transform.FindChildRecursive("QuantityAdjustmentContainer");
		_killQuantity = _quantityAdjustContainer.FindChildRecursive("Quantity_Text").GetComponent<Text>();
		_killQuantityText = _quantityAdjustContainer.FindChildRecursive("KillQuantity_Text").GetComponent<Text>();
		_killQuantitySlider = transform.FindChildRecursive("KillQuantity_Slider").GetComponent<Slider>();
		_goldRewardTotal = transform.FindChildRecursive("GoldRewardTotal_Text").GetComponent<Text>();
		_goldPerAction = transform.FindChildRecursive("GoldReward_Text").GetComponent<Text>();
		_decreaseGoldReward = transform.FindChildRecursive("DecreaseReward_Button").GetComponent<Button>();
		_increaseGoldReward = transform.FindChildRecursive("IncreaseReward_Button").GetComponent<Button>();
		_notEnoughGold = transform.FindChildRecursive("NotEnoughGold_Text").GetComponent<Text>();
	}

	public void SelectHeroForNewQuest(CastleUI ui) {

		ui.Deselect();
		StartCoroutine(GameManagerUI.ZoomOutAndCenterRoutine());

		SelectingAlertForQuest = true;
		HeroForQuest = ui.SelectedHero.ID;

	}

	public void SelectAlertForQuest(KingdomAlert alert) {

		alert.DeselectUI();
		StartCoroutine(GameManagerUI.ZoomOutAndCenterRoutine());

		SelectingHeroForQuest = true;
		AlertForQuest = alert.AlertID;
	}

	/// <summary>
	/// This will start the quest creator UI if both Hero and Alert are assigned.
	/// </summary>
	/// <param name="alert">The alert the hero is being assigned to.</param>
	public void AssignHeroToAlert(KingdomAlert alert) {

		if (HeroForQuest == null) {
			Debug.LogException(new Exception("A hero must be assigned before initiating a quest."));
		}

		AlertForQuest = alert.AlertID;
		alert.DeselectUI();
		StartCoroutine(GameManagerUI.ZoomOutAndCenterRoutine());
		SelectingAlertForQuest = false;
		SelectingHeroForQuest = false;

		StartQuestCreator(GameManager.Instance.Heroes[HeroForQuest],
						  GameManager.Instance.Alerts[AlertForQuest]);
	}

	public void StartQuest() {

		// We need to either get all the parameters here to assign to the quest object
		// and then assign to the hero, or we need to just assign the quest to the hero and send
		// that hero on its way. The latter would likely be easier.

		if (_questObject.GoldReward > GameManager.Instance.Gold) {
			StartCoroutine(NotEnoughGoldErrorRoutine());
			return;
		}

		Hero hero = GameManager.Instance.Heroes[HeroForQuest];
		KingdomAlert alert = GameManager.Instance.Alerts[AlertForQuest];

		GameManager.Instance.Gold -= _questObject.GoldReward;
		_questObject.Destination = alert.transform.position;
		//TODO: This will need to be set dynamically
		//TODO: Registering an event may work better than an enum.
		_questObject.Goal = QuestObject.QuestType.Slayer;
		//TODO: QuestObjects should be added to the list immediately or we won't be able to string tasks.
		hero.Quests.Add(_questObject);
		//TODO: This should go somewhere else.
		hero.gameObject.SetActive(true);
		hero.BeginQuest();

		CancelQuestBuilder();
	}

	private IEnumerator NotEnoughGoldErrorRoutine() {

		Color color = _notEnoughGold.color;
		color.a = 1;
		_notEnoughGold.color = color;
		float time = 5f;
		while (time > 0) {
			time -= Time.deltaTime;
			if (time < 2f) {
				color = _notEnoughGold.color;
				color.a = time / 2f;
				_notEnoughGold.color = color;
			}
			yield return null;
		}
	}

	/// <summary>
	/// Closes the quest builder UI and resets all values.
	/// </summary>
	public void CancelQuestBuilder() {

		// Zero out selected objects.
		AlertForQuest = new Guid();
		HeroForQuest = new Guid();
		_questObject = null;

		_questBuilderUI.gameObject.SetActive(false);
		_quantityAdjustContainer.gameObject.SetActive(false);
	}

	/// <summary>
	/// Activates the quest creator UI.
	/// </summary>
	/// <param name="hero">The assigned hero.</param>
	/// <param name="alert">The assigned alert.</param>
	private void StartQuestCreator(Hero hero, KingdomAlert alert) {

		_questObject = new QuestObject(hero, alert);
		Debug.Log("New blank quest object instantiated");

		// Make the UI visible.
		_questBuilderUI.SetActive(true);

		_questTitle.text = alert.AlertTitle;
		_questDesc.text = alert.AlertDescription;

		switch (alert.AlertType) {
			case KingdomAlert.AlertTypeEnum.EnemySpotted:
				SetUpEnemySpottedUI(hero, alert);
				break;
		}

	}

	private int GetRecommendedGoldReward(Hero hero, KingdomAlert alert) {

		float heroStat = 0f;
		switch (alert.UsedAttribute) {
			default:
				heroStat = hero.Strength;
				Debug.LogException(new Exception(alert.name +" did not have UsedAttribute set!"));
				break;
			case Attributes.Strength:
				heroStat = hero.Strength;
				break;
			case Attributes.Wisdom:
				heroStat = hero.Wisdom;
				break;
			case Attributes.Cunning:
				heroStat = hero.Cunning;
				break;
		}

		// The Mathf formula returns a value that represents easy at .9 and almost impossible at .01.
		// We subtract the value from 2 to get us a upward scaling modifier for our gold reward.
		float difficulty = 2f - Mathf.Clamp(heroStat / alert.Difficulty, .01f, .90f);
		// The reward is based on the heroes level and the difficulty of the task.
		float reward = hero.Level + (hero.Level * difficulty * difficulty);

		return Mathf.RoundToInt(reward);
	}

	public void IncreaseGoldReward() {

		int goldPerKill;
		if(Int32.TryParse(_goldPerAction.text, out goldPerKill)) {
			_goldPerAction.text = (goldPerKill + 1).ToString();
			UpdateTotalGoldReward();
		}
	}

	public void DecreaseGoldReward() {

		int goldPerKill;
		if (Int32.TryParse(_goldPerAction.text, out goldPerKill)) {
			if (goldPerKill >= 2) {
				_goldPerAction.text = (goldPerKill - 1).ToString();
				UpdateTotalGoldReward(); 
			}
		}
	}

	private void UpdateTotalGoldReward() {

		int goldPerKill;
		if (Int32.TryParse(_goldPerAction.text, out goldPerKill)) {
			_questObject.GoldReward = goldPerKill * _questObject.Quantity;
			_goldRewardTotal.text = "Gold Reward: " + _questObject.GoldReward.ToString();
		}
	}

	#region EnemySpotted methods

	private void SetUpEnemySpottedUI(Hero hero, KingdomAlert alert) {

		// Turn on the quantity adjustment widget
		_quantityAdjustContainer.gameObject.SetActive(true);
		int recommendedGold = GetRecommendedGoldReward(hero, alert);
		_goldPerAction.text = recommendedGold.ToString();
		_questObject.GoldReward = recommendedGold * alert.Quantity;
		_goldRewardTotal.text = "Gold Reward: " + recommendedGold * alert.Quantity;
		UpdateKillQuantityText(hero.Name, alert.Quantity, alert.EnemyName, 0);
		_killQuantity.text = alert.Quantity.ToString();
		_questObject.Quantity = alert.Quantity;
		// Assign new delegates to increase and decrease buttons so they can reference hero and alert.
		_killQuantitySlider.onValueChanged.AddListener(delegate {
			ChangeKillQuantity(hero, alert);
		});
		_killQuantitySlider.maxValue = alert.Quantity;
		_killQuantitySlider.value = alert.Quantity;
	}

	private void ChangeKillQuantity(Hero hero, KingdomAlert alert) {

		_questObject.Quantity = (int)_killQuantitySlider.value;
		UpdateKillQuantityText(
			hero.Name, _questObject.Quantity, alert.EnemyName, alert.Quantity - _questObject.Quantity);
		UpdateTotalGoldReward();
	}

	// Used to update text for kill style quests.
	private void UpdateKillQuantityText(string heroName, int killCount, string enemyName, int remaining) {

		_killQuantity.text = (_questObject.Quantity).ToString();
		_killQuantityText.text = string.Format(
			"Send {0} to slay {1} {2}? On success there will be {3} remaining.",
			heroName,
			killCount,
			enemyName,
			remaining);
	}

	#endregion
}