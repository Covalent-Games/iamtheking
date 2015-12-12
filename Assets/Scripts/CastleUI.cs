using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;


public class CastleUI : MonoBehaviour, ISelectable {

	private Hero _selectedHero;

	private Castle _castle;
	private Canvas _canvas;

	private Text _name;
	private Transform _heroList;
	private GameObject _heroIconPrefab;

	private Transform _heroInfoCanvas;
	private Text _heroLevel;
	private Image _heroLevelGauge;
	private Text _heroName;
	private Image _heroStrBar;
	private Image _heroWisBar;
	private Image _heroCunBar;

	void Awake() {

		_castle = GetComponent<Castle>();

		_canvas = transform.FindChildRecursive("Info_Canvas").GetComponent<Canvas>();
		_canvas.worldCamera = Camera.main;
		_canvas.enabled = false;
		// General UI
		_name = transform.FindChildRecursive("Name_text").GetComponent<Text>();
		_heroList = transform.FindChildRecursive("Content");
		_heroIconPrefab = (GameObject)Resources.Load("UIObjects/HeroIcon");
		// Hero Info UI
		_heroInfoCanvas = transform.FindChildRecursive("HeroInfo_Canvas");
		_heroInfoCanvas.gameObject.SetActive(false);
		_heroLevel = transform.FindChildRecursive("HeroLevel_Text").GetComponent<Text>();
		_heroLevelGauge = transform.FindChildRecursive("HeroLevelGauge_Image").GetComponent<Image>();
		_heroName = transform.FindChildRecursive("HeroName_Text").GetComponent<Text>();
		_heroStrBar = transform.FindChildRecursive("StrengthBar_Image").GetComponent<Image>();
		_heroWisBar = transform.FindChildRecursive("WisdomBar_Image").GetComponent<Image>();
		_heroCunBar = transform.FindChildRecursive("CunningBar_Image").GetComponent<Image>();
	}

	public void DisplayHero(Hero hero) {

		_selectedHero = hero;
		_heroInfoCanvas.gameObject.SetActive(true);
		_heroLevel.text = "Level: " + hero.Level;
		_heroLevelGauge.fillAmount = hero.Exp / hero.ExpToLevel;
		_heroName.text = hero.Name;
		_heroStrBar.fillAmount = hero.Strength;
		_heroWisBar.fillAmount = hero.Wisdom;
		_heroCunBar.fillAmount = hero.Cunning;
	}

	public void Select() {

		_canvas.enabled = true;

		GameObject heroIcon;
		foreach(Hero hero in GameManager.Instance.Heroes.Values) {
			if (hero.Idle) {
				heroIcon = Instantiate(_heroIconPrefab);
				// Parent the heroIcon to the scrollview Content object, adding it to the scrollable objects.
				heroIcon.transform.SetParent(_heroList);

				// This line is a workaround. If scaling issues happen with the icons try commenting this out.
				heroIcon.transform.localScale = new Vector3(1, 1, 1);
				// Update the icon's image to match the hero's image
				heroIcon.GetComponent<Image>().sprite = hero.UIImage;
				// Update the icon's banner/nametag to match the hero's name.
				heroIcon.transform.FindChildRecursive("HeroName").GetComponent<Text>().text = hero.Name;

				// Internal "syncing".
				heroIcon.name = hero.Name;
				heroIcon.GetComponent<HeroTavernSelection>().HeroID = hero.ID;
			}
		}
	}

	public void Deselect() {

		// This may not be efficient for GC, but it's the cleanest way I can think of and the list 
		// shouldn't have more than 5-10 at any time. Not worth caching the icons IMHO.

		// Remove all the hero icons from the scrollview.
		for (int i = 0; i < _heroList.childCount; i++) {
			Destroy(_heroList.GetChild(i).gameObject);
		}

		if (_selectedHero != null) {
			_heroInfoCanvas.gameObject.SetActive(false);
		}

		_canvas.enabled = false;
	}
}
