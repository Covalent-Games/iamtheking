using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Castle : MonoBehaviour, ISelectable {

	public float Integrity {
		get {
			return _integrity;
		}
		 set {
			_integrity = value;
			if (_integrity > _maxIntegrity) {
				_integrity = _maxIntegrity;
			}
			if (_integrity < 0) {
				GameManager.GameOver();
			}
			GameManager.Instance.UpdateCastleIntegrityUI(_integrity);
		}

	}
	private float _integrity;
	private float _maxIntegrity = 1f;


	void Start() {

		Integrity = _maxIntegrity;
	}

	public void DisplayHeroOnUI(Hero hero) {

		GameManager.Instance.KingdomCastleUI.DisplayHero(hero);
	}

	public void Select() {

		GameManager.Instance.KingdomCastleUI.Select();
	}

	public void Deselect() {

		GameManager.Instance.KingdomCastleUI.Deselect();
	}
}
