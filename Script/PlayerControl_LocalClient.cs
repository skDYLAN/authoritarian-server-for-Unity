using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerControl_LocalClient : NetworkBehaviour {


	float pingClient;

	// UI
	[SerializeField]
	GameObject Prefab_UI_LocalPlayer;
	GameObject UI_LocalPlayer;
	Text UI_tPing;
	Text UI_tTime;


	NetworkConnection connectToClient;

	public float _serverTime;


	// Use this for initialization
	void Start () {
		if(!isLocalPlayer)
			return;
	

		_serverTime = Time.time;
		CmdGetTimeToServer();
		InitUI_LocalPlayer();
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(!isLocalPlayer)
			return;
		
		UpdateUI_LocalPlayer();

	}

	void InitUI_LocalPlayer()
	{	
		
		StartCoroutine(GetPing());

		UI_LocalPlayer = Instantiate(Prefab_UI_LocalPlayer);
		if(UI_LocalPlayer != null)
		{
			UI_tPing = UI_LocalPlayer.GetComponent<Transform>().Find("tPing").GetComponent<Text>();
			UI_tTime = UI_LocalPlayer.GetComponent<Transform>().Find("tTime").GetComponent<Text>();
		}
	}

	void UpdateUI_LocalPlayer()
	{
		if(UI_tPing != null)
			UI_tPing.text = "Ping: " + pingClient.ToString();				
		if(UI_tTime != null)
			UI_tTime.text = "Time on Server: " + (_serverTime + Time.time);
	}

	IEnumerator GetPing()
    {
		if(isLocalPlayer)
		{
			while(true)
			{
				yield return new WaitForSeconds(3f);
				CmdSendTimeToServer(Time.time);
			}
		}
	}

	[Command]
	void CmdSendTimeToServer(float sendTime)
	{
		if(isServer)
			TargetSendTimeToServer(connectionToClient, sendTime);
	}

	[TargetRpc]
	void TargetSendTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			pingClient = Time.time-sendTime;
	}

	[Command] // выполняется на сервере
	void CmdGetTimeToServer()
	{	
		if(isServer)
		{
			TargetGetTimeToServer(connectionToClient, GameObject.Find("GameManager").GetComponent<NetworkGameManager>().serverTime);
			//GameObject.Find ("GameManager").GetComponent<NetworkManager_> ().serverTime2 = 1007;
		}
	}
	[TargetRpc] // вызываеися на клиенте
	void TargetGetTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			_serverTime = sendTime - Time.time + pingClient;
	}

}
