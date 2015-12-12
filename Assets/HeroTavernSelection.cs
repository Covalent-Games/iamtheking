using UnityEngine;
using System.Collections;

public class HeroTavernSelection : MonoBehaviour {

	public void SelectHero() {

		Debug.Log("Selected " + name + ". GameManager = " + GameManager.Instance.name);
	}

	public void Deselect() {

	}
}
