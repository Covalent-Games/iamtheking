using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;


internal class NameGenerator {

	#region Enemy name lists
	private static List<string> EnemyNames = new List<string>() {
		"Orc",
		"Goblin",
		"Spider",
		"Troll",
		"Warlock",
		"Zombie",
		"Skeleton",
	};
	#endregion
	#region Hero Name Lists
	private static List<string> PrefixList = new List<string>() {
		"",
		"",
		"",
		"Sir",
		"Lord",
		"Count",
		"Duke",
	};
	private static List<string> SuffixList = new List<string>() {
		"Jr",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"",
		"Sr",
		"II",
		"III",
		"IV",
		"V",
		"VI",
		"VII",
		"VIII",
		"IX",
		"X",
		"The Proud",
		"The Wary",
		"The Powerful",
		"Snaggle Tooth",
		"Long Beard",
		"Short Beard",
		"The Mighty",
		"The Watchful",
	};

	private static List<string> NameList = new List<string>() {
		"Queper",
		"Vuxity",
		"Kimves",
		"Itran",
		"Echech",
		"Quoelaw",
		"Dryurner",
		"Kelran",
		"Edeni",
		"Shyll",
		"Inelye",
		"Keliaine",
		"Kelad",
		"Angnys",
		"Osban",
		"Eara",
		"Chetor",
		"Yaech",
		"Honcer",
		"Etnban",
		"Emdana",
		"Nysquedar",
		"Nefuhu",
		"Ywdyna",
		"Cinysiss",
		"Ardque",
		"Ashoma",
		"Jim",
		"Rakch",
		"Iwore",
		"Nacoju",
		"Deyssul",
		"Emsay",
		"Wityto",
		"Hybod",
		"Gutyte",
		"Ashrdra",
		"Moper",
		"Nylorir",
		"Isstansam",
		"Radom",
		"Umoem",
		"Vesmostas",
		"Chaunde",
		"Anguh",
		"Lleoston",
		"Therashrod",
		"Hatmos",
		"Endsaye",
		"Nuin",
		"Ineem",
		"Dalus",
		"Aldgarom",
		"Sulip",
		"Engoump",
		"Orhina",
		"Unttsay",
		"Traichang",
		"Echbur",
		"Sammos",
		"Aldir",
		"Belray",
		"Urnton",
		"Rothos",
		"Engther",
		"Shycery",
		"Schygar",
		"Nalttia",
		"Nahila",
		"Woras",
		"Reaughpol",
		"Haeng",
		"Rhad",
		"Suill",
		"Chauban",
		"Imkela",
		"Kimathoef",
		"Ackach",
		"Nalilt",
		"Idelo",
		"Hinp",
		"Sulser",
		"Rekaurd",
		"Kimeldser",
		"Stryler",
		"Imero",
		"Uskage",
		"Rodardhat",
		"Denienda",
		"Baer",
		"Onough",
		"Kimtine",
		"Emen",
		"Elory",
		"Yiyed",
		"Teilves",
		"Nylrad",
		"Aldril",
		"Skelachshy",
		"Lorq",
		"Phiskel",
		"Urnswar",
		"Daler",
		"Leraf",
		"Aning",
		"Kimenth",
		"Ackild",
		"Nordsam",
		"Enrdel",
		"Treapet",
		"Onbel",
		"Ustmim",
		"Wehano",
		"Nyer",
		"Tinald",
		"Eary",
		"Raywunt",
		"Chedan",
		"Tinhin",
	};
	#endregion
	internal static string NewHeroName() {

		string prefix = GetPrefix();
		string name = GetName();
		string suffix = GetSuffix();

		return prefix + name + suffix;
	}

	internal static string NewEnemyName() {

		return EnemyNames[Random.Range(0, EnemyNames.Count)];
	}

	private static string GetSuffix() {

		string suffix = SuffixList[Random.Range(0, SuffixList.Count)];
		if (suffix != "") {
			suffix = " " + suffix;
		}
		return suffix;
	}

	private static string GetName() {

		string name = NameList[Random.Range(0, NameList.Count)];
		return name;
	}

	private static string GetPrefix() {

		string prefix = PrefixList[Random.Range(0, PrefixList.Count)];
		if (prefix != "") {
			prefix += " ";
		}
		return prefix;
	}
}