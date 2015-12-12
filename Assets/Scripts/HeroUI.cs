using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Extensions;
using UnityEngine.EventSystems;

public class HeroUI : MonoBehaviour, ISelectable {

	private Hero _hero;
	private Canvas _canvas;
	private Text _heroName;

	void Awake () {

		_hero = GetComponent<Hero>();
		_canvas = transform.FindChild("Info_Canvas").GetComponent<Canvas>();
		_heroName = transform.FindChildRecursive("Name_text").GetComponent<Text>();
	}

	void Start() {

		_heroName.text = _hero.Name;
	}

	public void Select() {

		_canvas.enabled = true;
		Debug.Log(name + " selected");
	}

	public void Deselect() {

		_canvas.enabled = false;
		Debug.Log(name + " deselected");
	}

	public void GetUI() {


	}
}
