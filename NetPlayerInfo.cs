using UnityEngine;
using System.Collections;

public class NetPlayerInfo : MonoBehaviour {
	
	int playerLevel = 0;
	int playerID = -1;
	string playerName = "";
	public UILabel nameLabel;
	PhotonPlayer netPlayer;
	
	public UISprite starOne;
	public UISprite starTwo;
	public UISprite starThree;
	
	int currentStars = 0;
	int currentColorLevel = 0;
	
	public GameNetworkManager netManager;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void StarAmountCheck()
	{
		currentColorLevel = (int)(playerLevel / 3);
		currentStars = playerLevel % 3 + 1;
		
		Color currentColor = starOne.color;
		currentColor = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
		switch (currentStars)
		{
			case 1:
				starOne.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
				starTwo.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
				starThree.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
			break;
			
			case 2:
				starOne.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
				starTwo.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
				starThree.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
			break;
			
			case 3:
				starOne.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
				starTwo.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
				starThree.color = new Color(currentColor.r, currentColor.g, currentColor.b, 255);
			break;
		}
	}
	
	Color DetermineColor()
	{
		return Color.black;
	}
	public void UpdateLevel(int inLevel)
	{
		playerLevel = inLevel;
	}
	
	public void SetPlayer(int inLevel, string inName, int inPlayerID, PhotonPlayer inPhotonPlayer)
	{
		playerLevel = inLevel;
		playerID = inPlayerID;
		playerName = inName;
		nameLabel.text = playerName;
		netPlayer = inPhotonPlayer;
	}
	
	public void ResetInfo(UISprite colorSprite)
	{
		playerLevel = 0;
		playerName = "Empty";
		nameLabel.text = "";
		playerID = -1;
		netPlayer = null;
		colorSprite.color = Color.white;
	}
	
	public bool CheckIsPlayer(string inIdentification)
	{
		string identifier = playerName + playerID.ToString();
		
		if (inIdentification == identifier)
			return true;
			
		return false;
	}
	
	public void GetProfile()
	{
		if (netPlayer != null)
			netManager.RequestPlayerProfile(netPlayer);
	}
	
	public bool IsEmpty()
	{
		if (netPlayer != null)
			return false;
			
		return true;
	}
	
	public int GetID()
	{
		return playerID;
	}
}
