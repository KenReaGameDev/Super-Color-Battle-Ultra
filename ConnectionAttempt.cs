using UnityEngine;
using System.Collections;

public class ConnectionAttempt : MonoBehaviour {

	float connectionAttemptTimer = 0;
	float connectionAttemptSecond = 0;
	
	int remaining = 0;
	public StartPlaying playGame;
	public UILabel timeOutLabel;
	string version = "alpha1";
	// Use this for initialization
	void Start () {
	
	}	
	
	void OnEnable()
	{
		connectionAttemptTimer = 0;
		connectionAttemptSecond = 0;
		remaining = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		connectionAttemptSecond += Time.deltaTime;
		
		if (!PhotonNetwork.connected && connectionAttemptSecond > 1)
		{
			connectionAttemptTimer += Time.deltaTime;
			connectionAttemptSecond = 0;
			PhotonNetwork.ConnectUsingSettings(version);
		}
		
		if (PhotonNetwork.connected)
			playGame.ConnectionReEstablished();
			
		remaining = (int)(10 - connectionAttemptTimer);		
		timeOutLabel.text = "Timeout in " + remaining.ToString();
		
		// Failed attempt to reconnect
		if (connectionAttemptTimer > 10 && !PhotonNetwork.connected)
		{
			PhotonNetwork.offlineMode = true;
			playGame.ReconnectScreenEnable();
			connectionAttemptTimer = 0;
			remaining = 0;
		}
	}
}
