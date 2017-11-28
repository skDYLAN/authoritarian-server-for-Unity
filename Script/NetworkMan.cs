using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfo
{
    public NetworkInstanceId id;
    public string ip;
    public List<GameObject> playerObjects = new List<GameObject>();
    public NetworkConnection conn;
    public string nickname;

    public PlayerInfo(NetworkConnection _conn)
    {
        conn = new NetworkConnection();
        conn = _conn;
    }

    public void AddGameObject(GameObject obj)
    {
        if (obj != null)
            playerObjects.Add(obj);
    }

}

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
	
}
