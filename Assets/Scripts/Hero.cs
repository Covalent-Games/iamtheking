using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Hero : MonoBehaviour {

	public bool Idle = true;
	public string Name;
	public List<QuestObject> Quests = new List<QuestObject>();
	public Sprite UIImage;

	void Awake () {

		UIImage = GetComponent<SpriteRenderer>().sprite;
		//TODO: Name generator;
		Name = "Sir Hacknslash";
	}
	
	void Update () {
		
		if (Quests.Count > 0) {
			transform.position = Vector3.MoveTowards(transform.position, Quests[0].Destination, Time.deltaTime);
		}
	}
}
