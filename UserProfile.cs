using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Xml.Serialization;
using System;

public class UserProfile : MonoBehaviour {



	// New Fashion Version Information
	public int money = 0;
	public int rank = 0;
	public string[] titleRank = new string[5] {"Hobbyiest", "Indie", "Fashionista", "Modelista", "Runway Superstar"};
	public string currentTop = "default";
	public string currentBottom = "default";
	public string currentPattern = "default";
	
	public string userName = "default";
	public int experience = 0;
	public int level = 0;
	public int experienceTNL = 0;
	public int placeFirst = 0;
	public int placeSecond = 0;
	public int placeThird = 0;
	public int placeFourth = 0;
	
	public float todaysAverage = 0;
	public float yesterdaysAverage = 0;
	public float[] weeklyAverage = new float[7] {0,0,0,0,0,0,0};
	public float weeklyAverageCombined = 0;
	public List<float> todaysScores = new List<float>();
	
	public int previousDay = 0;
	public int previousMonth = 0;
	
	public int todaysDay = 0;
	public int todaysMonth = 0;
	
	public int currentConsecutivePlayed = 0;
	public int currentConsecutiveWins = 0;
	public int consecutiveWins = 0;
	public int consecutivePlayed = 0;
	
	public int averagedPlayedDaily = 0;
	public int playedToday = 0;
	public int playedYesterday = 0;
	List<int> playedEachDay = new List<int>();
	public float totalTimePlayed = 0;
	
	bool noAds = false;
	bool firstTime = true;
	
	XmlTextReader xmlReader;
	XmlTextWriter xmlWriter;
	XmlDocument profileDoc;
	
	public UILabel currentName;
	public UILabel invalidUser;
	
	public GameObject profileCreator;
	
	// Use this for initialization
	void Start () {		
		Debug.Log(Application.persistentDataPath);
		todaysDay = DateTime.Now.Day;
		todaysMonth = DateTime.Now.Month;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnEnable()
	{
		//userName = "OnEnable";
		
		if (!System.IO.File.Exists(Application.persistentDataPath + "/Profiles/profile.xml"))
		{
			if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Profiles/"))
			{
				System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Profiles");
			}
			
			//userName = "NoFile";
			TextAsset tAsset = Resources.Load("Profiles/profile") as TextAsset;
			if (tAsset == null)
			{
				userName = "NullAsset";
				return;
			}
			var xmlString = tAsset.text;
			//userName = "ToString";
			File.WriteAllText(Application.persistentDataPath + "/Profiles/profile.xml", xmlString);		
			//userName = "WroteXML";	
		}
		
		if (!File.Exists(Application.persistentDataPath + "/Profiles/profile.xml"))
		{		
			//userName = "XMLNoExist";
		}
		else
		{
			//userName = "XMLEXIST";
		}	
	
			
		profileDoc = new XmlDocument();
		profileDoc.Load(Application.persistentDataPath + "/Profiles/profile.xml");
		//userName = "XMLLOAD";
		firstTime = XmlConvert.ToBoolean(profileDoc.DocumentElement.SelectSingleNode("firstTime").InnerText);
		
		if (firstTime)
			FirstTimeProfile();	
		else
			LoadProfile();	
			
		
		// Set photon name is player is already connected.
		PhotonNetwork.player.name = userName;
		//profileDoc.Save(Application.dataPath + "/Resources/Profiles/profile.xml");
		
		//xmlWriter = new XmlTextWriter(
		//xmlWriter.(Application.dataPath + "/Resources/Profiles/profile.xml", null);

	}
	
	void CreateXML()
	{

	}
	
	void FirstTimeProfile()
	{
		profileCreator.SetActive(true);
	}
	
	public void OnSubmitName()
	{
		userName = currentName.text;
		profileDoc.DocumentElement.SelectSingleNode("userName").InnerText = userName;
		
		if (userName.Length < 3 || userName.Length > 10 || !ProfinityChecker.CheckProfileName(userName))
		{
			InvalidUsername();
			return;
		}
		
		profileDoc.DocumentElement.SelectSingleNode("firstTime").InnerText = "false";
		profileDoc.Save(Application.persistentDataPath + "/Profiles/profile.xml");
		LoadProfile();
		profileCreator.SetActive(false);
		
	}
	
	void InvalidUsername()
	{
		invalidUser.gameObject.SetActive(true);
	}
	
	// Loads information that will be viewed in profile.
	void LoadProfile()
	{
		userName = profileDoc.DocumentElement.SelectSingleNode("userName").InnerText;
		
		if (!ProfinityChecker.CheckProfileName(userName) || userName.Length < 3 || userName.Length > 10)
		{
			FirstTimeProfile();
			InvalidUsername();
		}
		
		PhotonNetwork.player.name = userName;
		experience = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("experience").InnerText);
		level = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("level").InnerText);
		placeFirst = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("placeFirst").InnerText);
		placeSecond = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("placeSecond").InnerText);
		placeThird = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("placeThird").InnerText);
		placeFourth = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("placeFourth").InnerText);
		
		
		previousDay = (int)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode("previousDay").InnerText);
		previousMonth = (int)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode("previousMonth").InnerText);
		
		bool differentDate = false;
		if (todaysMonth > previousMonth || todaysDay > previousDay)
			differentDate = true;
			
		// Get Averages
		yesterdaysAverage = (float)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode("yesterdaysAverage").InnerText);
		todaysAverage = (float)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode("todaysAverage").InnerText);
		totalTimePlayed = (float)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode("timePlayed").InnerText);
		
		playedToday = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("playedToday").InnerText);
		playedYesterday = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("playedYesterday").InnerText);
		consecutiveWins = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("consecutiveWins").InnerText);
		consecutivePlayed = System.Convert.ToInt32(profileDoc.DocumentElement.SelectSingleNode("consecutivePlayed").InnerText);
		
		string playedString = profileDoc.DocumentElement.SelectSingleNode("playedEachDay").InnerText;
		string[] daysP = playedString.Split(',');
		
		for (int i = 0; i < daysP.Length; i++)
		{
			playedEachDay.Add(Convert.ToInt32(daysP[i]));
		}
		
		if (!differentDate && todaysAverage > 0)
			todaysScores.Add(todaysAverage);
			
		// Load each day of the week.
		for (int i = 0; i < 7; i++)
		{
			string xmlGet = "weeklyDay" + i.ToString();
			weeklyAverage[i] = (float)System.Convert.ToDouble(profileDoc.DocumentElement.SelectSingleNode(xmlGet).InnerText);
		}
		

		
		if (differentDate)
		{
			// add yesterday to weekly average, change today to yesterday
			for (int i = 6; i > 0; i--)
			{
				weeklyAverage[i] = weeklyAverage[i-1];
			}	
			
			weeklyAverage[0] = yesterdaysAverage;
			yesterdaysAverage = todaysAverage;		
			todaysAverage = 0;
			
			playedEachDay.Add(playedYesterday);
			playedYesterday = playedToday;
			playedToday = 0;
			
			playedString += "," + playedYesterday.ToString();
			// Updates playedEachDay if changed
			profileDoc.DocumentElement.SelectSingleNode("playedEachDay").InnerText = playedString;
			
		}
		

		CalculateDailyPlayed();
		CalculateWeeklyAverage();
		SaveProfile();
	}
	
	void SaveProfile()
	{
		profileDoc.Save(Application.persistentDataPath + "/Profiles/profile.xml");
	}
	
	
	void CalculateDailyPlayed()
	{
		int totalAdded = 0;
		
		// Determine average games played daily.
		foreach (int d in playedEachDay)
		{
			averagedPlayedDaily += d;
			totalAdded++;
		}
		
		if (averagedPlayedDaily > 0)
			averagedPlayedDaily = (int)(averagedPlayedDaily / totalAdded);
	}
	
	void CalculateWeeklyAverage()
	{
		int totalAdded = 0;
		
		// Determine total weekly average. (last 7 days played, not this current week).		
		for (int i = 0; i < 7; i++)
		{
			if (weeklyAverage[i] == 0)
				continue;
			
			weeklyAverageCombined += weeklyAverage[i];
			totalAdded++;
		}
		
		weeklyAverageCombined = weeklyAverageCombined / totalAdded;		
	}
	

	// Write information to XML upon change.
	public void UpdateXML()
	{
		Debug.Log("updating XML");
		if (profileDoc == null)
			ReloadXML();
		
		profileDoc.DocumentElement.SelectSingleNode("experience").InnerText = experience.ToString();
		profileDoc.DocumentElement.SelectSingleNode("level").InnerText = level.ToString();
		profileDoc.DocumentElement.SelectSingleNode("placeFirst").InnerText = placeFirst.ToString();
		profileDoc.DocumentElement.SelectSingleNode("placeSecond").InnerText = placeSecond.ToString();
		profileDoc.DocumentElement.SelectSingleNode("placeThird").InnerText = placeThird.ToString();
		profileDoc.DocumentElement.SelectSingleNode("placeFourth").InnerText = placeFourth.ToString();
		profileDoc.DocumentElement.SelectSingleNode("todaysAverage").InnerText = todaysAverage.ToString();
		profileDoc.DocumentElement.SelectSingleNode("yesterdaysAverage").InnerText = yesterdaysAverage.ToString();
		
		for (int i = 0; i < 7; i++)
		{
			profileDoc.DocumentElement.SelectSingleNode("weeklyDay" + i.ToString()).InnerText = weeklyAverage[i].ToString();		
		}
		
		profileDoc.DocumentElement.SelectSingleNode("previousDay").InnerText = todaysDay.ToString();
		profileDoc.DocumentElement.SelectSingleNode("previousMonth").InnerText = todaysMonth.ToString();
		profileDoc.DocumentElement.SelectSingleNode("timePlayed").InnerText = totalTimePlayed.ToString();
		
		profileDoc.DocumentElement.SelectSingleNode("playedToday").InnerText = playedToday.ToString();
		profileDoc.DocumentElement.SelectSingleNode("playedYesterday").InnerText = playedYesterday.ToString();
		profileDoc.DocumentElement.SelectSingleNode("averageDailyPlayed").InnerText = averagedPlayedDaily.ToString();
		
		// Update consecutive
		if (currentConsecutiveWins > consecutiveWins)
			profileDoc.DocumentElement.SelectSingleNode("consecutiveWins").InnerText = currentConsecutiveWins.ToString();
			
		if (currentConsecutivePlayed > consecutivePlayed)
			profileDoc.DocumentElement.SelectSingleNode("consecutivePlayed").InnerText = currentConsecutivePlayed.ToString();
		
		SaveProfile();
	}
	
	public void NewRoom()
	{
		currentConsecutivePlayed = 0;
		currentConsecutiveWins = 0;
	}
	
	void ReloadXML()
	{
		if (!System.IO.File.Exists(Application.persistentDataPath + "/Profiles/profile.xml"))
		{
			if (!System.IO.Directory.Exists(Application.persistentDataPath + "/Profiles/"))
			{
				System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Profiles");
			}
			
			TextAsset tAsset = Resources.Load("Profiles/profile") as TextAsset;
			if (tAsset == null)
			{
				return;
			}
			var xmlString = tAsset.text;
			File.WriteAllText(Application.persistentDataPath + "/Profiles/profile.xml", xmlString);		
		}
		else
		{
			profileDoc.Load(Application.persistentDataPath + "/Profiles/profile.xml");
		}
	}
	
	public void OnUpdateUserName()
	{
		userName = UIInput.current.label.text;
	}
	
	public void AddExperience(int inPlace, int inPlayers)
	{
		Debug.Log("adding experience");
		// Calculate experience. 
		int gainedExperience = (inPlayers + 1) - inPlace;
		experience += gainedExperience;
		
		switch(inPlace)
		{
			case 1:
				currentConsecutiveWins++;
				placeFirst++;
				break;
			case 2:
				currentConsecutiveWins = 0;
				placeSecond++;
				break;
			case 3:
				currentConsecutiveWins = 0;
				placeThird++;
				break;
			case 4:
				currentConsecutiveWins = 0;
				placeFourth++;
				break;
		}
		
		experienceTNL = 20 + (level * 5);
		if (experience > experienceTNL)
		{
			level++;
			// Send new info about player level.
			experience = experience - experienceTNL;
		}
		
		UpdateXML();
		Debug.Log("added experience");
	}
	
	public void UpdateStatistics(float inScore, float gameTimePlayed)
	{
		todaysScores.Add(inScore);
		currentConsecutivePlayed++;
		consecutivePlayed++;
		
		float totalScore = 0;
		int count = 0;
		foreach (float indexScore in todaysScores)
		{
			totalScore += indexScore;
			count++;
		}
		
		todaysAverage = totalScore / count;
		todaysAverage = (float)Math.Round((double)todaysAverage, 2);
		
		// Seconds Divided by 60 = minutes.
		totalTimePlayed += gameTimePlayed / 60;
		totalTimePlayed = (float)Math.Round((double)totalTimePlayed, 2);
	}
}
