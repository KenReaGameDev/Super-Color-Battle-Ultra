using UnityEngine;
using System.Collections;

public class NetInformation : MonoBehaviour {

	public UILabel labelOnline;
	public UILabel labelInGame;
	
	float pollTimer = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if ((pollTimer += Time.deltaTime) > 5)
		{
			labelOnline.text = "Online: " + PhotonNetwork.countOfPlayers.ToString();
			labelInGame.text = "In Game: " + PhotonNetwork.countOfPlayersInRooms.ToString();
		}
	}
}
