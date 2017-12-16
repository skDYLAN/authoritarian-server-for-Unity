using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class NetworkMan : NetworkManager {

	public GameObject ObjServerLogic;
	List<PlayerInfo> playerInfo = new List<PlayerInfo>();
	string playerNickname = "UnkownPlayer";
	
	// Use this for initialization
	void Start () {
		
	}
	
	public override void OnServerDisconnect(NetworkConnection conn)
	{
		DestroyPlayer(conn);
		NetworkServer.DestroyPlayersForConnection(conn);
	}

	public override void OnServerConnect(NetworkConnection conn)
    {
		playerInfo.Add(new PlayerInfo(conn));
		if(playerNickname != "")
			playerInfo[0].nickname = playerNickname;
    }

	//добавить объект к списку объектов игрока
	public void AddObjectToPlayer(NetworkInstanceId id, GameObject obj)
	{
		foreach (var player in playerInfo)
		{
			if(player.id == id)
			{
				player.AddGameObject(obj);
				break;
			}
		}
	}

	//уничтожить все объекты игрока на сервере
	public void DestroyPlayer(NetworkConnection conn)
	{
		if(conn != null)
		{
			foreach (var player in playerInfo)
			{
				if(player.conn == conn)
				{
					foreach(var obj in player.playerObjects)
						NetworkServer.Destroy(obj);
				}
			}
		}
	}

	public void EditPlayerId(NetworkConnection conn, NetworkInstanceId id)
	{
		foreach (var player in playerInfo)
		{
			if(player.conn == conn)
			{
				player.id = id;
				Debug.Log(player.nickname);
			}
		}
	}

	public string GetNickname()
	{
		return playerNickname;
	}
	
	public List<PlayerInfo> GetPlayerInfo()
	{
		return playerInfo;
	}

	public GameObject GetGMLPByObjOfPlayer(GameObject objPlayer)
	{
		GameObject[] GMLP = GameObject.FindGameObjectsWithTag("GM_LP");
		foreach(GameObject gmlp in GMLP)
		{
			GameObject pl = gmlp.GetComponent<PlayerControl_LocalClient>().player;
			if(pl == objPlayer)
				return gmlp;
		}
		return null;
	}
	public void kickPlayerById(int id)
	{
		foreach (var player in playerInfo)
			{
				if(player.id.Value == id)
				{
					player.conn.Disconnect();
				}
			}
	}
	
	public PlayerInfo GetPlayerByConn(NetworkConnection conn)
	{
		foreach (var player in playerInfo)
		{
			if(player.conn == conn)
				return player;
		}
		return null;
	}

	public void SetNicknameOfPlayerByConn(NetworkConnection conn, string newNickname)
	{
		foreach (var player in playerInfo)
		{
			if(player.conn == conn)
				player.nickname = newNickname;
		}
	}

}
