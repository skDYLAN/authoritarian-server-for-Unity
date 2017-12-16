using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class serverLogic : NetworkBehaviour {
	public class PlayerInfo 
	{
		public NetworkInstanceId id;
		public string ip;
		public List<GameObject> playerObjects = new List<GameObject>();
		public NetworkConnection conn;
		public string playerNickname;

		public PlayerInfo(NetworkInstanceId _id, string _ip, NetworkConnection _conn)
		{
			id = _id;
			ip = _ip;
			conn = new NetworkConnection();
			conn = _conn;
		}

		public void AddGameObject(GameObject obj)
		{
			if(obj != null)
				playerObjects.Add(obj);
		}

	}


	//List<PlayerInfo> playerInfo = new List<PlayerInfo>();
	public int count;
	public float timer;

	
}
