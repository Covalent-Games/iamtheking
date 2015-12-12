using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public class Castle : MonoBehaviour {

	public int Integrity {
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
		}

	}
	public int Gold = 0;

	private int _integrity;
	private int _maxIntegrity = 100;
	private CastleUI _castleUI;

	void Awake() {

		_castleUI = GetComponent<CastleUI>();
	}

	void Start() {

		Integrity = _maxIntegrity;
	}
}
