using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UIConsole : MonoBehaviour {

    Transform console;
    InputField input;
    RectTransform viewTextTransform;
    Text viewText;

	// Use this for initialization
	void Start () {
        console = GetComponent<Transform>().Find("Panel");
        input = console.Find("InputField").GetComponent<InputField>();
        viewTextTransform = console.Find("Scroll View").Find("Viewport").Find("textConsole").GetComponent<RectTransform>();
        viewText = viewTextTransform.GetComponent<Text>();
    }


    public void GetInputString(string inputString)
    {
        viewText.text += inputString + "\n";
        input.text = "";
        viewTextTransform.sizeDelta = viewTextTransform.sizeDelta + new Vector2(0, 16);
        Regex regCom = new Regex(".*=");
        Regex regParam = new Regex("=.*");
        Match matchCom = regCom.Match(inputString);
        Match matchParam = regParam.Match(inputString);
        string command = matchCom.Value;
        string param = matchParam.Value;

        if(command != "")
            command = command.Remove(command.Length-1);
        if(param != "")
            param = param.Remove(0,1);

        if(command != "" && param != "")
            ExecuteCommand(command, param, 1);
        else
            ExecuteCommand(inputString, "", 0);
    }

    void ExecuteCommand(string command, string param, int flag)
    {
        if(flag == 1)
        {
            switch (command)
            {
                case "kick":
                    {
                        kick_com(int.Parse(param));
                        break;
                    }
                case "nickname":
                    {
                        GameObject.Find("GM_local_main").GetComponent<ConsoleCmd>().CmdSetNickname(param);
                        break;
                    }
                default:
                    viewText.text += "Unkown Command" + "\n";
                    break;
            }
        }
        if(flag == 0)
            switch (command)
            {
                case "status":
                    {
                        status_com();
                        break;
                    }
                case "death":
                    {
                        GameObject.Find("GM_local_main").GetComponent<ConsoleCmd>().CmdDeathPlayer();
                        break;
                    }
                default:
                    viewText.text += "Unkown Command0" + "\n";
                    break;
            }

    } 
	
    void status_com()
    {
        GameObject[] gm_managers = GameObject.FindGameObjectsWithTag("GM_LP");
        foreach(GameObject gm in gm_managers)
        {
            viewText.text += "id:" + gm.GetComponent<NetworkIdentity>().netId.Value + " nick:" + gm.GetComponent<NetGM_Local>().playerNickname  + "\n";
            viewTextTransform.sizeDelta = viewTextTransform.sizeDelta + new Vector2(0, 16);
        }
    }

    void kick_com(int id)
    {
        GameObject.Find("GM_local_main").GetComponent<ConsoleCmd>().CmdKickPlayer(id);
        //  CmdKickPlayer(id);
    }   
}
