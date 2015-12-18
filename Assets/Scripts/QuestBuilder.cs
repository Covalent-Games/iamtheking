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
	private Transform _quantityAdjustContainer;
	private Text _killQuantity;
	private Text _killQuantityText;
	private Button _decreaseQuantityButton;
	private Button _increaseQuantityButton;


	private void Awake() {

		_questBuilderUI = transform.FindChildRecursive("QuestBuilder_Panel").gameObject;
		_questTitle = transform.FindChildRecursive("QuestTitle_Text").GetComponent<Text>();
		_questDesc = transform.FindChildRecursive("QuestDesc_Text").GetComponent<Text>();
		_quantityAdjustContainer = transform.FindChildRecursive("QuantityAdjustmentContainer");
		_killQuantity = _quantityAdjustContainer.FindChildRecursive("Quantity_Text").GetComponent<Text>();
		_killQuantityText = _quantityAdjustContainer.FindChildRecursive("KillQuantity_Text").GetComponent<Text>();
		_decreaseQuantityButton = transform.FindChildRecursive("DecreaseQuantity_Button").GetComponent<Button>();
		_increaseQuantityButton = transform.FindChildRecursive("IncreaseQuantity_Button").GetComponent<Button>();
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

		Hero hero = GameManager.Instance.Heroes[HeroForQuest];
		KingdomAlert alert = GameManager.Instance.Alerts[AlertForQuest];

		_questObject.Destination = alert.transform.position;
		//TODO: This will need to be set dynamically
		//TODO: Registering an event may work better than an enum.
		_questObject.Goal = QuestObject.QuestType.Slayer;
		//TODO: QuestObjects should be added to the list immediately or we won't be able to string tasks.
		hero.Quests.Add(_questObject);
		//TODO: This should go somewhere else.
		hero.gameObject.SetActive(true);
		hero.BeginQuest();

		Debug.Log(string.Format("{0} {1} is on the way to save the day!",
			GameManager.Instance.Alerts[AlertForQuest].AlertDescription,
			hero.Name));
		// This just does all the closing duties after we officially start the quest.
		CancelQuestBuilder();
	}

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
				// Turn on the quantity adjustment widget
				_quantityAdjustContainer.gameObject.SetActive(true);
				UpdateKillQuantityText(hero.Name, alert.Quantity, alert.EnemyName, 0);
				_killQuantity.text = alert.Quantity.ToString();
				_questObject.Quantity = alert.Quantity;
				// Assign new delegates to increase and decrease buttons so they can reference hero and alert.
				_decreaseQuantityButton.onClick.AddListener(delegate {
					DecreaseKillQuantity(hero, alert);
				});
				_increaseQuantityButton.onClick.AddListener(delegate {
					IncreaseKillQuantity(hero, alert);
				});
				break;
		}

	}

	// Increases the kill count and updates the text.
	public void IncreaseKillQuantity(Hero hero, KingdomAlert alert) {

		if (Int32.TryParse(_killQuantity.text, out _questObject.Quantity)) {
			if (_questObject.Quantity < alert.Quantity) {
				_questObject.Quantity++;
				_killQuantity.text = (_questObject.Quantity).ToString();
				UpdateKillQuantityText(
					hero.Name, _questObject.Quantity, alert.EnemyName, alert.Quantity - _questObject.Quantity);
			}
		}
	}

	// Decreases the kill count and updates the text.
	public void DecreaseKillQuantity(Hero hero, KingdomAlert alert) {

		if (Int32.TryParse(_killQuantity.text, out _questObject.Quantity)) {
			if (_questObject.Quantity -1 > 0) {
				_questObject.Quantity--;
				_killQuantity.text = (_questObject.Quantity).ToString();
				UpdateKillQuantityText(
					hero.Name, _questObject.Quantity, alert.EnemyName, alert.Quantity - _questObject.Quantity);
			}
		}
	}

	// Used to update text for kill style quests.
	private void UpdateKillQuantityText(string heroName, int killCount, string enemyName, int remaining) {

		_killQuantityText.text = string.Format(
			"Send {0} to slay {1} {2}? On success there will be {3} remaining.",
			heroName,
			killCount,
			enemyName,
			remaining);
	}
}