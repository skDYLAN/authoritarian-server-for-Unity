using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkGameManager : NetworkBehaviour {

	public float serverTime;
	// Use this for initialization
	void Start () {
		
	}
	
	[Server]
	// Update is called once per frame
	void Update () {
		serverTime = Time.time;
	}
}
