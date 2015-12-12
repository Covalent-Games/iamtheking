using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Extensions;

public class GameManagerUI : MonoBehaviour {

	public Image CastleIntegrityBar;
	public Text IdleHeroesText;

	void Awake() {

		CastleIntegrityBar = transform.FindChildRecursive("CastleIntegrity_Bar").GetComponent<Image>();
		IdleHeroesText = transform.FindChildRecursive("IdleHeroes_Text").GetComponent<Text>();
	}
}
