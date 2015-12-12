using UnityEngine;
using System;

public class HeroTavernSelection : MonoBehaviour {

	[SerializeField]
	public Guid HeroID;

	public void SelectHero() {

		Map.KingdomCastle.DisplayHeroOnUI(GameManager.Instance.Heroes[HeroID]);
	}
}
