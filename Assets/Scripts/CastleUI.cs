using System.Collections;
using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;


public class CastleUI : MonoBehaviour, ISelectable {

	public Hero SelectedHero;

	private GameObject _canvas;

	private Text _name;
	private Text _heroesInTavern;
	private Transform _heroList;
	private GameObject _heroIconPrefab;

	private Transform _heroInfoCanvas;
	private Text _heroLevel;
	private Image _heroLevelGauge;
	private Text _heroName;
	private Image _heroStrBar;
	private Image _heroWisBar;
	private Image _heroCunBar;
	private Image _heroAllegianceBar;
	private Text _heroAllegiancePer;

	private Button _startQuestButton;
	private Image _heroHealthBar;

	void Awake() {

		_canvas = transform.FindChildRecursive("CastleUI_Canvas").gameObject;
		// General UI
		_name = transform.FindChildRecursive("Name_text").GetComponent<Text>();
		_heroesInTavern = transform.FindChildRecursive("TavernTitle_Text").GetComponent<Text>();
		_heroList = transform.FindChildRecursive("Content");
		_heroIconPrefab = (GameObject)Resources.Load("UIObjects/HeroIcon");
		// Hero Info UI
		_heroInfoCanvas = transform.FindChildRecursive("HeroInfo");
		_heroInfoCanvas.gameObject.SetActive(false);
		_heroLevel = transform.FindChildRecursive("HeroLevel_Text").GetComponent<Text>();
		_heroLevelGauge = transform.FindChildRecursive("HeroLevelGauge_Image").GetComponent<Image>();
		_heroName = transform.FindChildRecursive("HeroName_Text").GetComponent<Text>();
		_heroStrBar = transform.FindChildRecursive("StrengthBar_Image").GetComponent<Image>();
		_heroWisBar = transform.FindChildRecursive("WisdomBar_Image").GetComponent<Image>();
		_heroCunBar = transform.FindChildRecursive("CunningBar_Image").GetComponent<Image>();
		_heroAllegianceBar = transform.FindChildRecursive("AllegianceBar_Image").GetComponent<Image>();
		_heroAllegiancePer = transform.FindChildRecursive("AllegiancePercent_Text").GetComponent<Text>();
		_heroHealthBar = transform.FindChildRecursive("HealthBar_Image").GetComponent<Image>();

		// Add listening to the start quest button to reference the selected hero properly.
		_startQuestButton = transform.FindChildRecursive("StartQuestCreator_Button").GetComponent<Button>();
		_startQuestButton.onClick.AddListener(
			delegate { GameManager.Instance.QuestCreator.SelectHeroForNewQuest(this); });
	}

	public void DisplayHero(Hero hero) {

		SelectedHero = hero;
		_heroInfoCanvas.gameObject.SetActive(true);
		_heroLevel.text = "Level: " + hero.Level;
		_heroLevelGauge.fillAmount = hero.Exp / hero.ExpToLevel;
		_heroName.text = hero.Name;
		_heroHealthBar.fillAmount = (float)hero.Health / (float)hero.MaxHealth;
		_heroStrBar.fillAmount = hero.Strength;
		_heroWisBar.fillAmount = hero.Wisdom;
		_heroCunBar.fillAmount = hero.Cunning;
		_heroAllegiancePer.text = ((int)(hero.Allegiance * 100)) + "%";
		_heroAllegianceBar.fillAmount = hero.Allegiance;
	}

	public void Select() {

		_canvas.SetActive(true);
		if (GameManager.Instance.IdleHeroCount > 0) {
			_heroesInTavern.text = "Heroes in the Tavern: " + GameManager.Instance.IdleHeroCount;
		} else {
			_heroesInTavern.text = "Your barkeeper isn't happy. He doesn't have any customers. But this is good news" +
				" for your kingdom. All your heroes are out on quests! However, you should consider recruiting more." +
				" [Coming Soon]";
		}
		PopulateHeroList(_heroList);
	}

	public void PopulateHeroList(Transform content) {

		//TODO: There's a bug associated with this... DON'T FORGET TO FIX IT!
		// Remove all the hero icons from the scrollview.
		for (int i = 0; i < _heroList.childCount; i++) {
			Destroy(_heroList.GetChild(i).gameObject);
		}

		GameObject heroIcon;
		foreach (Hero hero in GameManager.Instance.Heroes.Values) {
			if (hero.Idle) {
				heroIcon = Instantiate(_heroIconPrefab);
				// Parent the heroIcon to the scrollview Content object, adding it to the scrollable objects.
				heroIcon.transform.SetParent(content);

				// This line is a workaround. If scaling issues happen with the icons try commenting this out.
				heroIcon.transform.localScale = new Vector3(1, 1, 1);
				// Update the icons image to match the heros image
				heroIcon.GetComponent<Image>().sprite = hero.UIImage;
				// Update the icons banner/nametag to match the heros name.
				heroIcon.transform.FindChildRecursive("HeroName").GetComponent<Text>().text = hero.Name;

				// Internal "syncing".
				heroIcon.name = hero.Name;
				heroIcon.GetComponent<HeroTavernSelection>().HeroID = hero.ID;
			}
		}
	}

	public void Deselect() {

		// This may not be efficient for GC, but its the cleanest way I can think of and the list 
		// shouldnt have more than 5-10 at any time. Not worth caching the icons IMHO.

		if (SelectedHero != null) {
			_heroInfoCanvas.gameObject.SetActive(false);
		}

		_canvas.SetActive(false);
	}
}
