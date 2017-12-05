using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        viewTextTransform.sizeDelta = viewTextTransform.sizeDelta + new Vector2(0, 15);
    }
	
}
