using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetGM_Local : NetworkBehaviour {

	public float _serverTime;
	float pingClient;
	[SyncVar] public float myPing;
	[SyncVar] public string playerNickname;
	void Start()
	{	
		if(isServer)
			playerNickname = "UnkownPlayer";
			//playerNickname = GameObject.Find("Network").GetComponent<NetworkMan>().GetNickname();
			
		if(isLocalPlayer)
		{
			StartCoroutine(GetInfOfServer());
			gameObject.name = "GM_local_main";
		}
	}

	IEnumerator GetInfOfServer()
    {
		if(isLocalPlayer)
		{
			while(true)
			{
				CmdSendTimeToServer(Time.time);
                CmdGetTimeToServer();
                yield return new WaitForSeconds(1f);
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
			pingClient = Time.time-sendTime;
			CmdSendCurPingToServer(pingClient);
	}
	[Command]
	void CmdSendCurPingToServer(float _myPing) // синхронизация отображаемого пинга между клиентами
	{
		myPing = _myPing;
	}
	[Command] // выполняется на сервере
	void CmdGetTimeToServer()
	{	
		if(isServer)
		{
			TargetGetTimeToServer(connectionToClient, Time.time);
			//GameObject.Find ("GameManager").GetComponent<NetworkManager_> ().serverTime2 = 1007;
		}
	}
	[TargetRpc] // вызываеися на клиенте
	void TargetGetTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			_serverTime = sendTime - Time.time + pingClient;
	}
	
	public float GetPing()
	{
		return pingClient;
	}

	public float GetServerTime()
	{
		return _serverTime + Time.time;
	}

}
