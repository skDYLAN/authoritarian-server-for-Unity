using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class NetworkManager_ : NetworkBehaviour {
	
	static public List<PlayerNetwork> lPlyer = new List<PlayerNetwork>();
	static public NetworkManager_ sInstance = null;
	public GameObject guiZone;
	public Font font;

	// Use this for initialization
	void Start () {
		sInstance = this;


	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public override void OnStartClient()
    {
        base.OnStartClient();

	}
}
