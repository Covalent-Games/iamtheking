using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Extensions;
using System;
using System.Collections;

public class KingdomAlertUI : MonoBehaviour, ISelectable {

	private KingdomAlert _alert;
	private Canvas _canvas;
	private Text _alertTitle;
	private Text _alertQuantity;
	private Button _actionButton;
	private Text _alertDesc;

	private void Awake() {

		_alert = GetComponent<KingdomAlert>();
		_canvas = transform.FindChildRecursive("Info_Canvas").GetComponent<Canvas>();
		_canvas.enabled = false;
		_alertTitle = transform.FindChildRecursive("AlertTitle_Text").GetComponent<Text>();
		_alertDesc = transform.FindChildRecursive("AlertDesc_Text").GetComponent<Text>();
		_alertQuantity = transform.FindChildRecursive("AlertQuantity_Text").GetComponent<Text>();
		_actionButton = transform.FindChildRecursive("Action_Button").GetComponent<Button>();
	}

	private void Start() {

		_alertTitle.text = _alert.AlertTitle;
		_alertDesc.text = _alert.AlertDescription;
	}

	public void Select() {

		Hero hero;
		GameManager.Instance.Heroes.TryGetValue(QuestBuilder.HeroForQuest, out hero);

		// Set the button text based on context.
		// This is true if a hero has already been selected.
		if (QuestBuilder.SelectingAlertForQuest) {
			_actionButton.transform.FindChildRecursive("Text").GetComponent<Text>().text =
				"Assign " + hero.Name + " to this alert?";
			_actionButton.onClick.AddListener(
				delegate { GameManager.Instance.QuestCreator.AssignAlertToHero(_alert); });
		} else {
			_actionButton.transform.FindChildRecursive("Text").GetComponent<Text>().text =
				"Assign a hero to this alert?";
			_actionButton.onClick.AddListener(
				delegate { GameManager.Instance.QuestCreator.SelectAlertForQuest(_alert); });
		}

		switch (_alert.AlertType) {
			case KingdomAlert.AlertTypeEnum.EnemySpotted:
				_alertQuantity.text = 
					string.Format(_alert.QuantityTextTemplate, _alert.Quantity, _alert.EnemyName);
				_canvas.enabled = true;

				break;
		}
	}

	public void Deselect() {

		_canvas.enabled = false;
	}
}
