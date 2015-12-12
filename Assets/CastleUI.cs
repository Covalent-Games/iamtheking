using UnityEngine;
using UnityEngine.Extensions;
using UnityEngine.UI;


public class CastleUI : MonoBehaviour, ISelectable {

	private Castle _castle;
	private Canvas _canvas;
	private Text _name;
	private Transform _heroList;
	private GameObject _heroIconPrefab;

	void Awake() {

		_castle = GetComponent<Castle>();
		_canvas = transform.FindChildRecursive("Info_Canvas").GetComponent<Canvas>();
		_name = transform.FindChildRecursive("Name_text").GetComponent<Text>();
		_heroList = transform.FindChildRecursive("Content");
		_heroIconPrefab = (GameObject)Resources.Load("UIObjects/HeroIcon");
		_canvas.worldCamera = Camera.main;
	}

	public void Select() {

		_canvas.enabled = true;

		GameObject heroIcon;
		foreach(Hero hero in GameManager.Instance.Heroes) {
			if (hero.Idle) {
				heroIcon = Instantiate(_heroIconPrefab);
				heroIcon.transform.SetParent(_heroList);
				// This line is a workaround. If scaling issues happen with the icons try commenting this out.
				heroIcon.transform.localScale = new Vector3(1, 1, 1);
				heroIcon.GetComponent<Image>().sprite = hero.UIImage;
				heroIcon.transform.FindChildRecursive("HeroName").GetComponent<Text>().text = hero.Name;
			}
		}
	}

	public void Deselect() {

		for (int i = 0; i < _heroList.childCount; i++) {
			Destroy(_heroList.GetChild(i).gameObject);
		}
		_canvas.enabled = false;
	}
}
