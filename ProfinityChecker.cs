using UnityEngine;
using System.Collections;

public class ProfinityChecker {

	static string[] bannedWords = new string[77]{"anal","anus","arse","ass","ballsack","balls","bastard","bitch","biatch","bloody","blowjob","blowjob","bollock","bollok","boner","boob","bugger","bum","butt","buttplug","clitoris","cock","coon","crap","cunt","damn","dick","dildo","dyke","fag","feck","fellate","fellatio","felching","fuck","fuck","fudgepacker","fudgepacker","flange","Goddamn","Goddamn","hell","homo","jerk","jizz","knobend","knobend","labia","lmao","lmfao","muff","nigger","nigga","omg","penis","piss","poop","prick","pube","pussy","queer","scrotum","sex","shit","shit","sh1t","slut","smegma","spunk","tit","tosser","turd","twat","vagina","wank","whore","wtf"};
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/// <summary>
	/// Checks the name of the profile.	
	/// <returns><c>true</c>, if profile name was valid, <c>false</c> profanity was found.</returns>
	/// <param name="inName">In name.</param>
	/// </summary>
	public static bool CheckProfileName(string inName)
	{
		foreach (string badWord in bannedWords)
		{
			if (inName.Contains(badWord))
				return false;
		}
		
		return true;
	}
}
