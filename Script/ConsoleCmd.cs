using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ConsoleCmd : NetworkBehaviour {

    public GameObject UI_Chat;
    public string textChat;

    public void SetGameObjectChat(GameObject UIChat)
    {
        UI_Chat = UIChat;
    }

    void Update()
    {
        if(isLocalPlayer && textChat != "")
        {
            UI_Chat.GetComponent<UIChat>().InputMsg(textChat);
            textChat = "";
        }
    }

    [Command]
    public void CmdKickPlayer(int id)
    {
        GameObject.Find("Network").GetComponent<NetworkMan>().kickPlayerById(id);
    }
    // SendMsg
    [Command]
    public void CmdChatSendMsg(string msg)
    {
        PlayerInfo playerInf = GameObject.Find("Network").GetComponent<NetworkMan>().GetPlayerByConn(connectionToClient);
        RpcChatSendAllMsg("[" + playerInf.nickname + "]: " + msg);
    }
    [ClientRpc]
    void RpcChatSendAllMsg(string msg)
    {
        GameObject.Find("UI_Chat").GetComponent<UIChat>().InputMsg(msg);
    }
    // EndSendMsg

    [Command]
    public void CmdSetNickname(string nickname)
    {
        GetComponent<NetGM_Local>().playerNickname = nickname;
        GameObject.Find("Network").GetComponent<NetworkMan>().SetNicknameOfPlayerByConn(connectionToClient,nickname);
    }
    [Command]
    public void CmdDeathPlayer()
    {
        GetComponent<PlayerControl_LocalClient>().player.GetComponent<PlayerNetwork>().DestroyObject();
    }
}
