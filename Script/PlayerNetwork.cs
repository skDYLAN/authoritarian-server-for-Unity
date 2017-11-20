using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour {

	Text _textPosition;

    //Серверные значения
    public float ServerMoveHorizontal = 0;
    public float ServerMoveVertical = 0;
    public bool ServerJump = false;
    public float ServerYRotation = 0;
    public float ServerXRotation = 0;
    //

    //Старые Серверные значения
    float ServerMoveHorizontalOld = 0;
    public float ServerMoveVerticalOld = 0;
	bool ServerJumpOld = false;
	float ServerYRotationOld = 0;
	float ServerXRotationOld = 0; 
	//

	//Серверные
	public float maxRate = 0.03f;
    PlayerControl playerControl_server;
    //

    protected Rigidbody _rigidbody;
    protected Collider _collider;
	static public NetworkManager_ sInstance = null;
	protected bool _wasInit = false;
	public float tick=0;
	public float timeOfClient = 0;
	public float timeOfServer = 0;
	public float pingClient = 0;

	void Awake()
	{
		NetworkManager_.lPlyer.Add(this);
	}

	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
        
		

        //we MAY be awake late (see comment on _wasInit above), so if the instance is already there we init
            Init();
        if (isServer)
        {
            StartCoroutine(IEPlayerMove());
            playerControl_server = GetComponent<PlayerControl>();
        }
	}
	void Init()
	{
 		if (_wasInit)
            return;
		
		if(isLocalPlayer)
		{
			GameObject camControl = new GameObject("camControl");
			camControl.transform.SetParent(GetComponent<Transform>(), false);
			camControl.transform.localPosition = new Vector3(0f, 0.45f, 0f);
			camControl.AddComponent<Camera>();
		}
		/*
			GameObject textPositionObj = new GameObject("textPosition");
			textPositionObj.transform.SetParent(NetworkManager_.sInstance.guiZone.transform, false);
			_textPosition = textPositionObj.AddComponent<Text>();
			_textPosition.font = NetworkManager_.sInstance.font;
			_textPosition.color = Color.black;
		}
		 */

		_wasInit = true;

	}

	
	void FixedUpdate()
	{
		if(isServer)
		{
			if(ServerMoveHorizontal != ServerMoveHorizontalOld || ServerMoveVertical != ServerMoveVerticalOld )
			{
				ServerMoveHorizontalOld = ServerMoveHorizontal;
				ServerMoveVerticalOld = ServerMoveVertical;

                playerControl_server.setMoveHorizontal(ServerMoveHorizontal);
                playerControl_server.setMoveVertical(ServerMoveVertical);
			}
            playerControl_server.setRotationX(ServerXRotation);
            playerControl_server.setRotationY(ServerYRotation);
		}
	}

	[ClientRpc(channel=Channels.DefaultReliable)]
	void RpcMovePlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		if(isClient)
		{
			GetComponent<PlayerControl_Client>().SetNewPositionOfPlayer(newPosition, newRotation, timeScreen);
		}
	}


	[ClientRpc(channel=Channels.DefaultReliable)]
	void RpcplayerRotation(Quaternion rotationCurrent)
	{
		if(isServer || isLocalPlayer)
			return;
			
			GetComponent<PlayerControl>().playerRotationSlerp(rotationCurrent);
		
		
	}

	IEnumerator IEPlayerMove()
    {
		tick++;
		if(isServer)
		{
			while(true)
			{
				RpcMovePlayer(transform.position, transform.rotation, Time.time);
				yield return new WaitForSeconds(maxRate);
			}
		}
	}


}
