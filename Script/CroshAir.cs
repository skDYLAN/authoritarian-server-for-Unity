using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroshAir : MonoBehaviour {

	public Texture2D pricel;

	public void OnGUI ()
	{
		GUI.DrawTexture (new Rect (Screen.width / 2, Screen.height / 2, 20, 20), pricel);
	}

}
