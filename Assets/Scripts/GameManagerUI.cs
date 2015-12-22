using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Extensions;
using System;
using System.Linq;
using System.Collections;

public class GameManagerUI : MonoBehaviour {

	public GameManager Manager;
	public Image CastleIntegrityBar;
	public Text IdleHeroesText;
	public Text KingdomPopulation;
	public Text Gold;

	void Awake() {

		CastleIntegrityBar = transform.FindChildRecursive("CastleIntegrity_Bar").GetComponent<Image>();
		IdleHeroesText = transform.FindChildRecursive("IdleHeroes_Text").GetComponent<Text>();
		KingdomPopulation = transform.FindChildRecursive("KingdomPop_Text").GetComponent<Text>();
		Gold = transform.FindChildRecursive("Gold_Text").GetComponent<Text>();
	}

	internal void UpdateIdleHeroCount(int idleHeroCount) {

		// While this is normally bad practice, there shouldnt be more than 10-15 heroes ever.
		Hero[] idleHeroes = Manager.Heroes.Values.Where(hero => hero.Idle).ToArray();
		if (idleHeroes.Length != idleHeroCount) {
			Manager.IdleHeroCount = idleHeroes.Length;
			IdleHeroesText.text = "Idle Heroes: " + idleHeroCount.ToString();
		}
	}

	internal void UpdateGold(int gold) {

		Gold.text = "Gold: " + gold.ToString();
	}

	internal void UpdatePopulation(int population) {

		KingdomPopulation.text = "Kingdom Population: " + population.ToString();
	}

	public static IEnumerator ZoomOutAndCenterRoutine() {

		Vector3 targetPos = new Vector3(0f, 0f, Camera.main.transform.position.z);
		Vector3 zoomVelocity = Vector3.zero;
		float orthoVelocity = 0f;
		int frameDelayToAcceptTouch = 0;
		while (Camera.main.transform.position != targetPos) {
			frameDelayToAcceptTouch++;
			if (frameDelayToAcceptTouch > 20 && Input.touchCount > 0) {
				break;
			}
			Camera.main.transform.position = Vector3.SmoothDamp(
				Camera.main.transform.position,
				targetPos,
				ref zoomVelocity,
				0.25f);
			Camera.main.orthographicSize = Mathf.SmoothDamp(
				Camera.main.orthographicSize,
				15f,
				ref orthoVelocity,
				0.2f);
			yield return null;
		}
	}
}
