using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestObject {

	public Vector3 Destination;
	public QuestType Goal;
	public int Quantity;
	public int GoldReward;

	public enum QuestType {
		Slayer,
	}
}
