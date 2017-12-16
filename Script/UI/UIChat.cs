using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChat : MonoBehaviour {

	Transform console;
    InputField input;
    RectTransform viewTextTransform;
    Text viewText;
	ConsoleCmd consoleCmd;

	void Start () {
        console = GetComponent<Transform>().Find("Chat");
        input = console.Find("InputField").GetComponent<InputField>();
        viewTextTransform = console.Find("Scroll View").Find("Viewport").Find("textChat").GetComponent<RectTransform>();
        viewText = viewTextTransform.GetComponent<Text>();
    }
	public void GetInputString(string inputString)
    {
		GameObject.Find("GM_local_main").GetComponent<ConsoleCmd>().CmdChatSendMsg(inputString);
        input.text = "";
	}
	public void InputMsg(string msg)
	{
			viewText.text += msg + "\n";
	}
}
