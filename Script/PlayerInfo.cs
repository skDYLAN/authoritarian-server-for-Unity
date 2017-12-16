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
