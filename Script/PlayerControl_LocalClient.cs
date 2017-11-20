using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerControl_LocalClient : NetworkBehaviour {


	float pingClient;

	// UI
	[SerializeField]
	GameObject Prefab_UI_LocalPlayer;
	GameObject UI_LocalPlayer;
	Text UI_tPing;
	Text UI_tTime;
    Text UI_tTail;
    Text UI_tDelta;

    GameObject camControl;

    // вводимые значения игроком
    float _moveHorizontal = 0;
    float _moveVertical = 0;
    bool _jump = false;
    float _yRotation = 0;
    float _xRotation = 0;
    //
    //старые значения
    float _moveHorizontalOld = 0;
    float _moveVerticalOld = 0;
    bool _jumpOld = false;
    float _yRotationOld = 0;
    float _xRotationOld = 0;
    //


    NetworkConnection connectToClient;
    PlayerNetwork playerNetwork;

    public float _serverTime;



	// Use this for initialization
	void Start () {
		if(!isLocalPlayer)
			return;

       // playerNetwork = GetComponent<PlayerNetwork>();

        InitCamControll();
		InitUI_LocalPlayer();

		
	}

    private void Update()
    {
        if (isLocalPlayer)
        {
            _moveVertical = 0;
            if (Input.GetKey(KeyCode.W))
                _moveVertical = 1;
            if (Input.GetKey(KeyCode.S))
                _moveVertical = -1;

            _moveHorizontal = 0;
            if (Input.GetKey(KeyCode.D))
                _moveHorizontal = 1;
            if (Input.GetKey(KeyCode.A))
                _moveHorizontal = -1;

            _jump = Input.GetButtonDown("Jump");

            _yRotation += Input.GetAxisRaw("Mouse Y");
            _xRotation += Input.GetAxisRaw("Mouse X");

            GetComponent<PlayerControl>().setRotationX(_xRotation);
            GetComponent<PlayerControl>().setRotationY(_yRotation);

            CmdSetRotationY_Server(_yRotation);
            CmdSetRotationX_Server(_xRotation);

            if (_moveHorizontal != _moveHorizontalOld)
            {
                _moveHorizontalOld = _moveHorizontal;
                CmdSendInputMoveHorizaontal(_moveHorizontal);
            }

            if (_moveVertical != _moveVerticalOld)
            {
                _moveVerticalOld = _moveVertical;
                CmdSendInputMoveVertical(_moveVertical);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
		if(!isLocalPlayer)
			return;
		
		UpdateUI_LocalPlayer();
        camControl.transform.position = transform.position + new Vector3(0f, 0.8f, 0f);
        camControl.transform.rotation = transform.rotation;

    }

	void InitUI_LocalPlayer()
	{	
		
		StartCoroutine(GetPing());

		UI_LocalPlayer = Instantiate(Prefab_UI_LocalPlayer);
		if(UI_LocalPlayer != null)
		{
			UI_tPing = UI_LocalPlayer.GetComponent<Transform>().Find("tPing").GetComponent<Text>();
			UI_tTime = UI_LocalPlayer.GetComponent<Transform>().Find("tTime").GetComponent<Text>();
            UI_tTail = UI_LocalPlayer.GetComponent<Transform>().Find("tTail").GetComponent<Text>();
            UI_tDelta = UI_LocalPlayer.GetComponent<Transform>().Find("tDelta").GetComponent<Text>();
        }
	}

    void InitCamControll()
    {
        camControl = new GameObject("camControl");
       // camControl.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        camControl.AddComponent<Camera>();
     }


    void UpdateUI_LocalPlayer()
	{
		if(UI_tPing != null)
			UI_tPing.text = "Ping: " + pingClient.ToString();				
		if(UI_tTime != null)
			UI_tTime.text = "Time on Server: " + (_serverTime + Time.time);
        if (UI_tTime != null)
            UI_tTail.text = "Tail: " + GetComponent<PlayerControl_Client>()._scrinsTransformPlayer.Count.ToString();
        if (UI_tDelta != null)
            UI_tDelta.text = "Delta: " + GetComponent<PlayerControl_Client>().deltaResult.ToString();

    }

	IEnumerator GetPing()
    {
		if(isLocalPlayer)
		{
			while(true)
			{
				CmdSendTimeToServer(Time.time);
                _serverTime = Time.time;
                CmdGetTimeToServer();
                yield return new WaitForSeconds(3f);
            }
		}
	}

	[Command]
	void CmdSendTimeToServer(float sendTime)
	{
		if(isServer)
			TargetSendTimeToServer(connectionToClient, sendTime);
	}

	[TargetRpc]
	void TargetSendTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			pingClient = Time.time-sendTime;
	}

	[Command] // выполняется на сервере
	void CmdGetTimeToServer()
	{	
		if(isServer)
		{
			TargetGetTimeToServer(connectionToClient, Time.time);
			//GameObject.Find ("GameManager").GetComponent<NetworkManager_> ().serverTime2 = 1007;
		}
	}

	[TargetRpc] // вызываеися на клиенте
	void TargetGetTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			_serverTime = sendTime - Time.time + pingClient;
	}

    [Command(channel = Channels.DefaultReliable)]
    void CmdSetRotationX_Server(float xRotation)
    {
        GetComponent<PlayerNetwork>().ServerXRotation = xRotation;
    }

    [Command(channel = Channels.DefaultReliable)]
    void CmdSetRotationY_Server(float yRotation)
    {
        GetComponent<PlayerNetwork>().ServerYRotation = yRotation;
    }

    [Command]
    void CmdSendInputMoveHorizaontal(float InputMove)
    {
        GetComponent<PlayerNetwork>().ServerMoveHorizontal = InputMove;
    }
    [Command]
    void CmdSendInputMoveVertical(float InputMove)
    {
        GetComponent<PlayerNetwork>().ServerMoveVertical = InputMove;
    }
}
