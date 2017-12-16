using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerControl_LocalClient : NetworkBehaviour {
	[SerializeField]  GameObject prefab_player;
	public GameObject player; // Таблетка 
	// UI
	[SerializeField] GameObject Prefab_UI_LocalPlayer;
    [SerializeField] GameObject Prefab_UI_arms_LocalPlayer;
    [SerializeField] GameObject Prefab_UI_tab_panel;
    [SerializeField] GameObject Prefab_UI_console;
    [SerializeField] GameObject UI_chat;
    GameObject UI_LocalPlayer;
    GameObject UI_Console;
    GameObject UI_arms_LocalPlayer;
    GameObject UI_tab_panel;
    Text UI_tPing;
	Text UI_tTime;
    Text UI_tTail;
    Text UI_tLoosePocket;
    Text UI_tHealth;

    GameObject cam_Scene;
    GameObject camControl;
    NetGM_Local netGM_Local;
    List <GameObject> objectsOfPlayer = new List<GameObject>(); // объекты игрока


    // вводимые значения игроком
    float _moveHorizontal = 0;
    float _moveVertical = 0;
    bool _jump = false;
    float _yRotation = 0;
    float _xRotation = 0;
    public bool _shot = false;
    //
    //старые значения
    float _moveHorizontalOld = 0;
    float _moveVerticalOld = 0;
    bool _jumpOld = false;
    float _yRotationOld = 0;
    float _xRotationOld = 0;
    bool _shotOld = false;
    //

    //Опережающий ввод
    float timeStartH = 0;
    float timeStartV = 0;
    float timeMoveVertical = 0;
    float timeMoveHorizonta = 0;
    public bool moveV;
    public bool moveH;


	// Use this for initialization
	void Start () {
		
        //serverLogic.AddPlayerInfo(netId, "", null);

        if(isLocalPlayer)
        {
            netGM_Local = GetComponent<NetGM_Local>();
            CmdInitPlayer(netId);
            cam_Scene = GameObject.Find("Cam_scene");
        }
	
	}

    private void Update()
    {
        if (isLocalPlayer)
        {
            // Блок управления
            if(Input.GetKeyDown(KeyCode.G) && player == null)
	    	{
                cam_Scene.SetActive(false);
		    	CmdSpawn(netId);
	    	}

            if(player != null)
            {
                if (UI_Console.activeInHierarchy == false)
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

                    if (Input.GetKeyDown(KeyCode.Space))
                        _jump = true;
                    if (Input.GetKeyUp(KeyCode.Space))
                        _jump = false;

                    _yRotation += Input.GetAxisRaw("Mouse Y");
                    _xRotation += Input.GetAxisRaw("Mouse X");

                    player.GetComponent<PlayerControl>().setRotationX(_xRotation);
                    player.GetComponent<PlayerControl>().setRotationY(_yRotation);

                    CmdSetRotationY_Server(_yRotation, player);
                    CmdSetRotationX_Server(_xRotation, player);

                    if (_moveHorizontal != _moveHorizontalOld)
                    {
                        _moveHorizontalOld = _moveHorizontal;
                        CmdSendInputMoveHorizaontal(_moveHorizontal, player);
                        moveH = false;
                        timeMoveHorizonta = Time.time + 0.1f;
                        timeStartH = Time.time + 0.1f + GetComponent<NetGM_Local>().GetPing() / 2;
                    }

                    if (_moveVertical != _moveVerticalOld)
                    {
                        _moveVerticalOld = _moveVertical;
                        CmdSendInputMoveVertical(_moveVertical, player);
                        moveV = false;
                        timeMoveVertical = Time.time + 0.1f;
                        timeStartV = Time.time + 0.1f + GetComponent<NetGM_Local>().GetPing() / 2;

                    }
                    if (_jump != _jumpOld)
                    {
                        _jumpOld = _jump;
                        CmdSendInputJump(_jump, player);
                    }

                    _shot = Input.GetButton("Fire1");


                    if (_shot !=_shotOld)
                    {
                        _shotOld = _shot;
                        CmdFire(_shot, player);
                    }

                    if (Time.time > timeStartH)
                        player.GetComponent<PlayerControl>().setMoveHorizontal(_moveHorizontalOld);
                    else
                        moveH = false;

                    if (Time.time > timeStartV)
                        player.GetComponent<PlayerControl>().setMoveVertical(_moveVerticalOld);
                    else
                        moveV = false;
                    
                    if(Input.GetKeyDown(KeyCode.T))
                        chatControl();

                }

               // CmdFire(true, player);
                //

                // блок вывод UI
                UpdateUI_LocalPlayer();
                UI_arms_LocalPlayer.GetComponent<Transform>().position = player.GetComponent<Transform>().position;
                UI_arms_LocalPlayer.GetComponent<Transform>().rotation = player.GetComponent<Transform>().rotation;
            }
            if (Input.GetKeyDown(KeyCode.F12))
                    ConsoleControl();

            if (Input.GetKeyDown(KeyCode.Tab) && UI_Console.activeInHierarchy == false)
                        tubControl(true);
            if (Input.GetKeyUp(KeyCode.Tab))
                        tubControl(false);
        }
    }

    // Update is called once per frame

    void chatControl()
    {
        /*
        if(UI_chat.activeSelf)
            UI_chat.SetActive(false);
        else
            UI_chat.SetActive(true);
             */

    }

    void tubControl(bool active)
    {
        if(UI_tab_panel != null)
            if(active)
            {
                UI_tab_panel.SetActive(true);
                GameObject[] players = GameObject.FindGameObjectsWithTag("GM_LP");
                NetworkInstanceId[] id = new NetworkInstanceId[players.Length];
                string[] nickName = new string[players.Length];
                float[] ping = new float[players.Length];
                for (int i=0; i < players.Length; i++)
                {
                    id[i] = players[i].GetComponent<NetGM_Local>().netId;
                    nickName[i] = players[i].GetComponent<NetGM_Local>().playerNickname;
                    ping[i] = players[i].GetComponent<NetGM_Local>().myPing;
                }
                UI_tab_panel.GetComponent<Transform>().Find("Panel").GetComponent<TabPanel>().DisplayTanPanel(id, nickName, ping);
            }
            else
                UI_tab_panel.SetActive(false);
    }

    void ConsoleControl()
    {
        if (UI_Console != null)
            if (UI_Console.activeSelf)
            {
                UI_Console.SetActive(false);
            }
            else
                UI_Console.SetActive(true);
    }

    [Command]
    void CmdInitPlayer(NetworkInstanceId _netId)
    {
        NetworkMan netMan = GameObject.Find("Network").GetComponent<NetworkMan>();
        netMan.EditPlayerId(connectionToClient, _netId);
        netMan.AddObjectToPlayer(_netId, gameObject);
    }
    [Command]
	void CmdSpawn(NetworkInstanceId _netId)
	{
		var obj = (GameObject)Instantiate(
			prefab_player, 
			transform.position + new Vector3(0,1,0), 
			Quaternion.identity);

        NetworkServer.Spawn(obj);

        NetworkMan netMan = GameObject.Find("Network").GetComponent<NetworkMan>();
        netMan.AddObjectToPlayer(_netId, obj);
        obj.GetComponent<PlayerNetwork>().idGM = _netId;

        TargetGetObj(connectionToClient, obj);
	}
	[TargetRpc]
	void TargetGetObj(NetworkConnection target, GameObject obj)
	{
		if(!isClient)
            return;
    
		player = obj;
        //objectsOfPlayer.Add(obj);
        InitUI_LocalPlayer();
	}
    
	void InitUI_LocalPlayer()
	{	
        //player.GetComponent<PlayerControl_Client>().SetObjGM(gameObject);
        player.GetComponent<PlayerControl_Client>().SetLolcalPlayer(true);

        UI_arms_LocalPlayer = Instantiate(Prefab_UI_arms_LocalPlayer);
        UI_LocalPlayer = Instantiate(Prefab_UI_LocalPlayer);
        UI_tab_panel = Instantiate(Prefab_UI_tab_panel);
        UI_Console = Instantiate(Prefab_UI_console);
        
        UI_chat = GameObject.Find("GM_Helper").GetComponent<GM_Helper>().UI_chat;
        if(UI_chat != null)
            UI_chat.SetActive(true);



		if(UI_LocalPlayer != null)
		{
			UI_tPing = UI_LocalPlayer.GetComponent<Transform>().Find("tPing").GetComponent<Text>();
			UI_tTime = UI_LocalPlayer.GetComponent<Transform>().Find("tTime").GetComponent<Text>();
            UI_tTail = UI_LocalPlayer.GetComponent<Transform>().Find("tTail").GetComponent<Text>();
            UI_tLoosePocket = UI_LocalPlayer.GetComponent<Transform>().Find("tLoosePocket").GetComponent<Text>();
            UI_tHealth = UI_LocalPlayer.GetComponent<Transform>().Find("tHealth").GetComponent<Text>();
        }
        if (UI_arms_LocalPlayer != null)
        {
            UI_arms_LocalPlayer.GetComponent<Transform>().Find("Camera").localPosition = new Vector3(0, 0.9f, 0);
            UI_arms_LocalPlayer.GetComponent<Transform>().Find("Gun").localPosition = new Vector3(0.613f, 0.55f, 0.458f);
        }
        if(UI_tab_panel != null)
        {
            UI_tab_panel.SetActive(false);
        }
        if (UI_Console != null)
            UI_Console.SetActive(false);

        player.GetComponent<Transform>().Find("Gun").GetComponent<MeshRenderer>().enabled = false;
        player.GetComponent<Transform>().Find("Hair").GetComponent<MeshRenderer>().enabled = false;
        player.GetComponent<Transform>().Find("Glass").GetComponent<MeshRenderer>().enabled = false;
        player.GetComponent<Transform>().Find("Capsule").GetComponent<MeshRenderer>().enabled = false;

    }

    void InitCamControll()
    {
        camControl = new GameObject("camControl");
       // camControl.transform.localPosition = new Vector3(0f, 0.45f, 0f);
        camControl.AddComponent<Camera>();
     }


    void UpdateUI_LocalPlayer()
	{
		if(UI_tPing != null && netGM_Local != null)
			UI_tPing.text = "Ping: " + netGM_Local.GetPing().ToString();				
		if(UI_tTime != null && netGM_Local != null)
			UI_tTime.text = "Time on Server: " + (netGM_Local.GetServerTime());
            
        if (UI_tTime != null)
            UI_tTail.text = "Tail: " + player.GetComponent<PlayerControl_Client>()._scrinsTransformPlayer.Count.ToString();
        if (UI_tLoosePocket != null)
            UI_tLoosePocket.text = "Loose pocket: " + player.GetComponent<PlayerControl_Client>().tickDel.ToString();
        if (UI_tHealth != null)
            UI_tHealth.text = "Health: " + player.GetComponent<PlayerNetwork>().health.ToString();

    }

    [Command(channel = Channels.DefaultReliable)]
    void CmdSetRotationX_Server(float xRotation, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerXRotation = xRotation;
    }

    [Command(channel = Channels.DefaultReliable)]
    void CmdSetRotationY_Server(float yRotation, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerYRotation = yRotation;
    }
    [Command]
    void CmdFire(bool fire, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerShout = fire;
    }
    [Command]
    void CmdSendInputMoveHorizaontal(float InputMove, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerMoveHorizontal = InputMove;
    }
    [Command]
    void CmdSendInputMoveVertical(float InputMove, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerMoveVertical = InputMove;
    }
    [Command]
    void CmdSendInputJump(bool InputJump, GameObject player)
    {
        player.GetComponent<PlayerNetwork>().ServerJump = InputJump;
    }

    public void DeathPlayer()
    {
        Destroy(UI_LocalPlayer);
        Destroy(UI_arms_LocalPlayer);
        cam_Scene.SetActive(true);
        player = null;
    }

    void OnDestroy()
	{
        Destroy(player);
        Destroy(UI_LocalPlayer);
        Destroy(UI_arms_LocalPlayer);
	}
}
