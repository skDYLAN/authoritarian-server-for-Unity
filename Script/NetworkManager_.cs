using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkManager_ : NetworkBehaviour {
	
	static public List<PlayerNetwork> lPlyer = new List<PlayerNetwork>();
	static public NetworkManager_ sInstance = null;
	public float serverTime = 1;

	// Use this for initialization
	void Start () {
		sInstance = this;
		if(isLocalPlayer)
			CmdGetTimeToServer(GetComponent<NetworkIdentity>());
	}
	
	// Update is called once per frame
	public override void OnStartClient()
    {
        base.OnStartClient();
		GetComponent<NetworkManager> ();
	}

	[Command] // выполняется на сервере
	void CmdGetTimeToServer(NetworkIdentity identy)
	{	
		if(isServer)
		{
			RpcGetTimeToServer(serverTime);
		}
	}
	[ClientRpc] // вызываеися на клиенте
	void RpcGetTimeToServer( float sendTime)
	{
		if(isClient)
			serverTime = sendTime;
	}
}
