using UnityEngine;
using System;

public class QuestBuilder: MonoBehaviour {

	public static bool SelectingAlertForQuest = false;
	public static bool SelectingHeroForQuest = false;
	public static Guid HeroForQuest;
	public static Guid AlertForQuest;

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
		Debug.Log(GameManager.Instance.Alerts[AlertForQuest].AlertTitle + " has been selected.");
	}

	public void AssignHeroToAlert(KingdomAlert alert) {

		AlertForQuest = alert.AlertID;
		alert.DeselectUI();
		StartCoroutine(GameManagerUI.ZoomOutAndCenterRoutine());
		SelectingAlertForQuest = false;
		SelectingHeroForQuest = false;

		Debug.Log(string.Format("{0} {1} is on the way to save the day!",
			GameManager.Instance.Alerts[AlertForQuest].AlertTitle,
			GameManager.Instance.Heroes[HeroForQuest].Name));
	}
}