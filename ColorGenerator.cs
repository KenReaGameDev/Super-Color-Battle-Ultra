using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class ColorGenerator : MonoBehaviour {
	
	public UISprite colorBox;
	public UISprite previewBox;
	
	public UILabel labelRed;
	public UILabel labelGreen;
	public UILabel labelBlue;
	public UILabel labelTimer;
	public UILabel labelDeviation;
	public UILabel labelSyncing;
	
	public UISlider sliderRed;
	public UISlider sliderBlue;
	public UISlider sliderGreen;	
	
	public ColorPreviewer previewer;
	
	public PhotonView photonView;
	public GameNetworkManager netManager;
	
	float timeInGame = 0;
	
	public enum PlayingState {
		PLAYING,
		SCORING,
		WAITING,
		SYNCING,
	};
	
	/// <summary>
	/// Table for scoring
	/// <playername + ID, ScoringInfo>
	/// </summary>
	public Dictionary<string, ScoringInformation> scoringTable = new Dictionary<string, ScoringInformation>(4);	
	List<ScoringInformation> currentScores = new List<ScoringInformation>(4);
	
	public UILabel[] nameLabelArray = new UILabel[4];
	public UILabel[] scoreLabelArray = new UILabel[4];
	public UIWidget[] widgetArray = new UILabel[4];
	public UIWidget scoreBoxBackground;
	
	int scoresThisRound = 1;
	int scoresTop;
	int scoresBottom;
	
	// RPC for syncing scores
	string[] keyArraySorted = new string[4];
	float[] scoreArraySorted = new float[4];
	int[] sortedIDs = new int[4];
	
	public PlayingState currentState = PlayingState.SYNCING;
	
	float playingTimer = 0;
	float scoringTimer = 0;
	float syncTimer = 0;
	
	bool sorted = false;
	bool synced = false;
	bool failedToSync = false;
	
	public UserProfile userInfo;
	
	// Scorebox
	public GameObject scoreBox;
	
	// Use this for initialization
	void Start () {
	

	}
	
	void OnEnable()
	{
		// TODO: Should I have put this as scoringTable.add? i don't think so.
		for (int i = 0; i < 4; i++)
		{
			currentScores.Add(new ScoringInformation());
		}		
		
		currentState = PlayingState.SYNCING;
		ScoringInformation myInformation = new ScoringInformation();
		myInformation.name = PhotonNetwork.player.name;
		myInformation.ID = PhotonNetwork.player.ID;
		myInformation.player = PhotonNetwork.player;
		string myKey = myInformation.name + myInformation.ID.ToString();
		scoringTable.Add(myKey, myInformation);
		sorted = true;
		ResetLabels();
	}
	
	// Update is called once per frame
	void Update () {
	
		timeInGame += Time.deltaTime;
		
		switch(currentState)
		{
			case PlayingState.PLAYING:
				PlayingUpdate();
				break;
			case PlayingState.SCORING:
				ScoringUpdate();
				break;
			case PlayingState.SYNCING:
				SyncingUpdate();
				break;
		}
	}
	
	byte RGB()
	{
		return (byte)UnityEngine.Random.Range(0,255);
	}
	
	void PlayingUpdate()
	{
		labelSyncing.gameObject.SetActive(false);
		playingTimer += Time.deltaTime;
		
		// Convert to non decimal seconds.
		int timerConversion = 10 - (int)playingTimer;
		labelTimer.text = timerConversion.ToString();
		
		if (playingTimer > 10)
		{	
			scoringTimer = 0;
			CalculateScore();
			currentState = PlayingState.SCORING;
			scoreBox.SetActive(true);
			sorted = false;
		}
	}
	
	void SyncingUpdate()
	{
		labelSyncing.gameObject.SetActive(true);
		//UnityEngine.AndroidJNI.
		if (PhotonNetwork.isMasterClient)
		{
			scoringTimer = 0;
			CalculateScore();
			currentState = PlayingState.SCORING;
			scoreBox.SetActive(true);
			sorted = false;		
		}
		
		if (!PhotonNetwork.isMasterClient)
		{
			Debug.Log("waiting for sync");
			syncTimer += Time.deltaTime;
			
			int randomroomnum =UnityEngine.Random.Range(0,9999999);
			if (syncTimer > 20)
			{
				PhotonNetwork.LeaveRoom();				
				syncTimer = 0;
				failedToSync = true;
			}
			
			if (failedToSync && syncTimer > 3)
			{
				PhotonNetwork.CreateRoom("random" + randomroomnum.ToString(), true, true, 4); 
				failedToSync = false;
			}
				
		}
	}
	
	void ScoringUpdate()
	{
		if (previewer.roundsUnchanged > 6)
		{
			Debug.Log("Disconnecting due to UnchangedRounds");
			PhotonNetwork.Disconnect();
		}
		
		labelSyncing.gameObject.SetActive(false);
		scoringTimer += Time.deltaTime;
		
		int timerConversion = 7 - (int)scoringTimer;
		labelTimer.text = timerConversion.ToString();
		
		// Wait for scores to come in then display scores.
		if (scoringTimer > 1 && !sorted && PhotonNetwork.isMasterClient)
		{
			sorted = true;
			SortScores();
			//DisplayScores();
		}
		
//		if (!PhotonNetwork.isMasterClient)
//			Debug.Log("Not Master Client");
			
		if (scoringTimer > 7 && PhotonNetwork.isMasterClient)
		{
			Color32 newColor = new Color32(RGB (), RGB (), RGB (), 255);
			colorBox.color = newColor;
			Vector3 sendColor = new Vector3(colorBox.color.r, colorBox.color.g, colorBox.color.b);
			
			
			photonView.RPC("SyncNewColor", PhotonTargets.Others, sendColor);
			Debug.Log("Sending SyncNewColor");
			
			previewer.ResetColors();
			ResetSliders();
			//labelDeviation.text = "";
			playingTimer = 0;			
			labelRed.text = "RED";
			labelBlue.text = "BLUE";
			labelGreen.text = "GREEN";
			scoreBox.SetActive(false);
			currentState = PlayingState.PLAYING;
			ResetLabels();
			synced = true;
		}		
	}
	
	void ResetLabels()
	{
		for (int i = 0; i < 4; i++)
		{
			nameLabelArray[i].text = "";
			scoreLabelArray[i].text = "";
		}
	}
	
	string ReturnName(int inID)
	{
		foreach (PhotonPlayer p in PhotonNetwork.playerList)
		{
			if (p.ID == inID)
				return p.name;
		}
		
		return null;
	}
	

	
	void AchievementChecker(string index)
	{
		switch(index)
		{
			case "99Percent":
				
			break;
		
			case "25Losses":
			
			if (PlayGamesPlatform.Instance.GetAchievement("CgkIvv-kp7MYEAIQCQ").IsUnlocked)
				break;
				
			PlayGamesPlatform.Instance.IncrementAchievement(
				"CgkIvv-kp7MYEAIQCQ", 1, (bool success) => {
				Debug.Log("25 Losses Unlocked");
			});
			
			break;
		
		}
	}
	
	/// <summary>
	/// Sorts the scores.
	/// </summary>
	void SortScores()
	{
		int currentPlace = 1;
		int currentPlayerCount = scoringTable.Count;
		scoresThisRound = currentPlayerCount;
		

			
		foreach (ScoringInformation playerScore in scoringTable.Values)
		{
		
			if (playerScore.score > 99)
				AchievementChecker("99Percent");
				
			foreach (ScoringInformation rivalScore in scoringTable.Values)
			{
				if (playerScore == rivalScore)
					continue;
					
				if (playerScore.score == rivalScore.score)
				{
					float chance = UnityEngine.Random.Range(-0.001f, 0.001f);
					playerScore.score += chance;
				}
				
				if (playerScore.score < rivalScore.score)
					currentPlace++;
					

			}
			
			playerScore.place = currentPlace;
			currentScores[playerScore.place - 1] = playerScore;
			
			if (playerScore.place < 1)
				AchievementChecker("25Losses");
				
			// For Sending RPC to other players
			scoreArraySorted[currentPlace - 1] = playerScore.score;
			keyArraySorted[currentPlace - 1] = playerScore.name + playerScore.ID.ToString();
			sortedIDs[currentPlace - 1] = playerScore.ID;
			currentPlace = 1;
		}
		
		if (currentPlayerCount < 4)
		{
			for (int i = currentPlayerCount; i < 4; i++)
			{
				currentScores[i] = new ScoringInformation();
				sortedIDs[i] = -1;
			}
		}
		
		Debug.Log("Sending SendScoreList");
		photonView.RPC("SendScoreList", PhotonTargets.All, scoreArraySorted, keyArraySorted);
		
	}
	
	void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
	{
		Debug.Log("Removing " + otherPlayer.name);
		scoringTable.Remove(otherPlayer.name + otherPlayer.ID);
	}
	
	// Recieves score list in sorted order and changes currentScores
	[RPC] void SendScoreList (float[] sortedScores, string[] sortedKeys)
	{
		Debug.Log("Recieving Send Score List");
		for (int i = 0; i < sortedKeys.Length; i++)
		{
			Debug.Log("key " + i);
			
			if (sortedKeys[i] == null || sortedKeys[i] == "")
				continue;
			
			ScoringInformation scoreInfo;
			Debug.Log(sortedKeys[i]);
			
			if (!scoringTable.TryGetValue(sortedKeys[i], out scoreInfo))
			{
				Debug.Log("sorted key " + sortedKeys[i] + " does not exist");
				continue;
			}
			
			string myPlayerKey = PhotonNetwork.player.name + PhotonNetwork.player.ID.ToString();
			if (sortedKeys[i] == myPlayerKey)
			{
				Debug.Log("adding exp");
				userInfo.UpdateStatistics(sortedScores[i], timeInGame);
				timeInGame = 0;
				if (PhotonNetwork.playerList.Length > 1)
					userInfo.AddExperience(i + 1, sortedKeys.Length);
				
			}
			
			scoringTable[sortedKeys[i]].place = i + 1;
			scoringTable[sortedKeys[i]].score = sortedScores[i];
			Debug.Log(scoringTable[sortedKeys[i]].score);
			currentScores[i] = scoringTable[sortedKeys[i]];
		}
		
		scoresThisRound = sortedKeys.Length;
		DisplayScores();
	}
	
	void DisplayScores()
	{
		//int playersInGame = PhotonNetwork.playerList.Length;
		scoresTop = 80;
		scoresBottom = scoreBoxBackground.height;
		// Set scores to their respective text fields.
		for (int i = 0; i < scoresThisRound; i++)
		{
			nameLabelArray[i].text = currentScores[i].name;
			currentScores[i].score = (float)System.Math.Round((double)currentScores[i].score, 2, System.MidpointRounding.ToEven);
			scoreLabelArray[i].text = currentScores[i].score.ToString() + "%";
		}
		
		// Clear all other Boxes
		if (scoresThisRound < 4)
		{
			//int remainerBoxes = 4 - scoresThisRound;
			
			for (int i = scoresThisRound - 1; i < 4; i++)
			{
				nameLabelArray[i].text = "";
				scoreLabelArray[i].text = "";
			}
		}		
		
		Debug.Log(currentScores.ToString());
		Debug.Log(scoringTable.ToStringFull());
		// Resize boxes so players can see who won.
		int usableSpace = scoresBottom - scoresTop;
		Debug.Log("Usable Space" + usableSpace);
		// Gets size 
		int labelSize = (usableSpace / scoresThisRound); //- 5;
		
		// Places labels where they need to be.
		for (int i = 0; i < scoresThisRound; i++)
		{
			widgetArray[i].topAnchor.absolute = (-labelSize * i) - 80;
			widgetArray[i].bottomAnchor.absolute = widgetArray[i].topAnchor.absolute - labelSize;
		}
		
	}
	

	// Host recieves score information from each player
	[RPC] void SendScoreToHost(float inScore, int playerID, string playerName)
	{			
		Debug.Log("SendScoreToHost");
		string key = playerName + playerID.ToString();
		ScoringInformation info = null;
		
		// Adds new info if player doesn't exist in score yet.
		if(!scoringTable.TryGetValue(key, out info))
		{
			CleanTable();
			info = new ScoringInformation();
			info.name = playerName;
			info.ID = playerID;		
			info.score = 0;
			info.place = 5;
			
			foreach (PhotonPlayer p in PhotonNetwork.playerList)
			{
				if (p.ID == playerID)
					info.player = p;
			}	
			
			scoringTable.Add(key, info);
			photonView.RPC("NewScoreInformation", PhotonTargets.Others, key, playerID);
			Debug.Log("Sending NewScoreInformation - SendScoreToHost");
		}
		
		info.score = inScore;		
	}
	
	[RPC] void NewScoreInformation(string key, int ID)
	{
		Debug.Log("NewScoreInformation");
		if (ID == PhotonNetwork.player.ID)
			return;
			
		ScoringInformation info = new ScoringInformation();
		CleanTable();
		info = new ScoringInformation();
		
		foreach (PhotonPlayer p in PhotonNetwork.playerList)
		{
			if (p.ID == ID)
			{
				info.name = p.name;
				info.player = p;	
			}
		}
		
		info.ID = ID;		
		info.score = 0;
		info.place = 5;
		
		Debug.Log("Adding " + key + " to table");
		scoringTable.Add(key, info);
	}
	
	void CleanTable()
	{	
		List<string> deletePlayers = new List<string>();
		foreach (ScoringInformation si in scoringTable.Values)
		{
			if (si.player == null)
			{
				si.markedForDeletion = true;
				deletePlayers.Add(si.name + si.ID.ToString());
			}
		}		
		
		foreach (string s in deletePlayers)
			scoringTable.Remove(s);
	}
	
	void OnPhotonPlayerConnected(PhotonPlayer otherPlayer)
	{
		// Send my information to the new player to add me into score.
		string myKey = PhotonNetwork.player.name + PhotonNetwork.player.ID.ToString();
		photonView.RPC("NewScoreInformation", PhotonTargets.Others, myKey, PhotonNetwork.player.ID);
		Debug.Log("Sending NewScoreInfomration -- OnPhotonPlayerConnected");
	}
	
	[RPC] void SyncNewColor(Vector3 inColor)
	{	
		Debug.Log("SyncNewColor");
		int red = (int)(inColor.x * 255);
		int green = (int)(inColor.y * 255);
		int blue = (int)(inColor.z * 255);
		
		Color32 newColor = new Color32((byte)red, (byte)green, (byte)blue, 255);
		colorBox.color = newColor;
		
		previewer.ResetColors();
		ResetSliders();
		//labelDeviation.text = "";
		playingTimer = 0;
		scoringTimer = 0;		
		labelRed.text = "RED";
		labelBlue.text = "BLUE";
		labelGreen.text = "GREEN";
		ResetLabels();
		
		scoreBox.SetActive(false);
		currentState = PlayingState.PLAYING;
		synced = true;
	}
	
	void ResetSliders()
	{
		sliderRed.value = 0.0f;
		sliderBlue.value = 0.0f;
		sliderGreen.value = 0.0f;
	}
	
	void CalculateScore()
	{
		int redActual = (int)(colorBox.color.r * 255);
		int redCompare = Mathf.Abs(redActual - previewer.previewRed);
		
		int greenActual = (int)(colorBox.color.g * 255);
		int greenCompare = Mathf.Abs(greenActual - previewer.previewGreen);
		
		int blueActual = (int)(colorBox.color.b * 255);
		int blueCompare = Mathf.Abs(blueActual - previewer.previewBlue);
		
		Debug.Log("Red: " + redActual + " | " + previewer.previewRed );
		Debug.Log("Green: " + greenActual + " | " + previewer.previewGreen );
		Debug.Log("Blue: " + blueActual + " | " + previewer.previewBlue );
		
		float deviation = redCompare + blueCompare + greenCompare;
		float correctPoints = 765 - deviation;
		float percentage = correctPoints / 765;
		float movedPercentage = percentage * 100;
		movedPercentage = (float)System.Math.Round((double)movedPercentage, 2, System.MidpointRounding.ToEven);
		
		if (!PhotonNetwork.isMasterClient)
		{
			Debug.Log("Sending host scores" + synced.ToString());
			
			if (!synced)			
				photonView.RPC("SendScoreToHost", PhotonTargets.MasterClient, 0.0f, PhotonNetwork.player.ID, PhotonNetwork.player.name);
			else
				photonView.RPC("SendScoreToHost", PhotonTargets.MasterClient, movedPercentage, PhotonNetwork.player.ID, PhotonNetwork.player.name);
				
			Debug.Log("Sending SendScoreToHost");
		}
		else
		{
			SendScoreToHost(movedPercentage, PhotonNetwork.player.ID, PhotonNetwork.player.name);
		}
		//Debug.Log(movedPercentage);
		
		//TODO: Clamp string
		//labelDeviation.text = movedPercentage.ToString() + "%";
	}
	
	public void RemoveFromTable(string identification)
	{
		if (scoringTable.ContainsKey(identification))
			scoringTable.Remove(identification);
	}
	
	public void ResetScores()
	{	
		sorted = false;
		synced = false;
		failedToSync = false;
		playingTimer = 0;
		syncTimer = 0;
		scoringTimer = 0;
		currentScores.Clear();
		scoringTable.Clear();
	}
}
