using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Hero : MonoBehaviour {

	public Guid ID;
	public bool Idle = true;
	public string Name;
	public ClassTypeEnum ClassType = ClassTypeEnum.Warrior;
	public List<QuestObject> Quests = new List<QuestObject>();
	public Sprite UIImage;
	public float Allegiance = 0.5f;
	public float Strength = 0.1f;
	public float Wisdom = 0.1f;
	public float Cunning = 0.1f;
	internal int Level = 1;
	internal float Exp = 0f;
	internal float ExpToLevel = 2f;

	public enum ClassTypeEnum {
		Warrior,
		Mage,
		Rogue,
	}

	void Awake () {

		ID = Guid.NewGuid();
		UIImage = GetComponent<SpriteRenderer>().sprite;
		//TODO: Name generator;
		string[] names = new string[5] {
			"Hacker",
			"Smacker",
			"Wacker",
			"Cracker",
			"Bob",
		};
		Name = names[UnityEngine.Random.Range(0, 4)];
		Strength = UnityEngine.Random.value;
		Wisdom = UnityEngine.Random.value;
		Cunning = UnityEngine.Random.value;
		Exp = UnityEngine.Random.value + UnityEngine.Random.value;
		Allegiance = UnityEngine.Random.value;
	}
	
	void Update () {
		
		if (Quests.Count > 0) {
			transform.position = Vector3.MoveTowards(transform.position, Quests[0].Destination, Time.deltaTime);
		}
	}
}
