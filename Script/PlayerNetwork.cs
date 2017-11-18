using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerNetwork : NetworkBehaviour {

	Text _textPosition;

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

	//Серверные значения
	float ServerMoveHorizontal = 0;
    public float ServerMoveVertical = 0;
	bool ServerJump = false;
	float ServerYRotation = 0;
	float ServerXRotation = 0; 
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
		if(isServer)
			StartCoroutine(IEPlayerMove());
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

	void Update () {
		if(isLocalPlayer)
		{
			_moveVertical = 0;
			if(Input.GetKey(KeyCode.W))
				_moveVertical = 1;
			if(Input.GetKey(KeyCode.S))
				_moveVertical = -1;
				
			_moveHorizontal = 0;
			if(Input.GetKey(KeyCode.D))
				_moveHorizontal = 1;
			if(Input.GetKey(KeyCode.A))
				_moveHorizontal = -1;

			_jump = Input.GetButtonDown("Jump");

			_yRotation += Input.GetAxisRaw("Mouse Y");
			_xRotation += Input.GetAxisRaw("Mouse X");

			GetComponent<PlayerControl>().setRotationX(_xRotation);
			GetComponent<PlayerControl>().setRotationY(_yRotation);
			

			timeOfClient = Time.time;
		}
		if(isServer)
			timeOfServer = Time.time;
	}

	void FixedUpdate()
	{
		if(isLocalPlayer)
		{
			CmdSetRotationY_Server(_yRotation);
			CmdSetRotationX_Server(_xRotation);

			if(_moveHorizontal != _moveHorizontalOld)
			{
				_moveHorizontalOld = _moveHorizontal;
				CmdSendInputMoveHorizaontal(_moveHorizontal);
				//StartCoroutine(ReturnToLoby(0.1f));
			}

			if(_moveVertical != _moveVerticalOld)
			{
				_moveVerticalOld = _moveVertical;
				CmdSendInputMoveVertical(_moveVertical);
				//StartCoroutine(ReturnToLoby(0.1f));
			}
		}

		if(isServer)
		{
			if(ServerMoveHorizontal != ServerMoveHorizontalOld || ServerMoveVertical != ServerMoveVerticalOld )
			{
				ServerMoveHorizontalOld = ServerMoveHorizontal;
				ServerMoveVerticalOld = ServerMoveVertical;
				
				GetComponent<PlayerControl>().setMoveHorizontal(ServerMoveHorizontal);
				GetComponent<PlayerControl>().setMoveVertical(ServerMoveVertical);
			}
			GetComponent<PlayerControl>().setRotationX(ServerXRotation);
			GetComponent<PlayerControl>().setRotationY(ServerYRotation);
		}
	}
	[Command]
	void CmdSendInputMoveHorizaontal(float InputMove)
	{
		ServerMoveHorizontal = InputMove;
	}
	[Command]
	void CmdSendInputMoveVertical(float InputMove)
	{
		ServerMoveVertical = InputMove;
	}

	[ClientRpc(channel=Channels.DefaultReliable)]
	void RpcMovePlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		if(isClient)
		{
			GetComponent<PlayerControl_Client>().SetNewPositionOfPlayer(newPosition, newRotation, timeScreen);
		}
	}

	[Command(channel=Channels.DefaultReliable)]
	void CmdSetRotationX_Server(float xRotation)
	{
		ServerXRotation = xRotation;
	}

	[Command(channel=Channels.DefaultReliable)]
	void CmdSetRotationY_Server(float yRotation)
	{
		ServerYRotation = yRotation;
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


	[Command]
	void CmdSendTimeToServer(float sendTime)
	{
		TargetSendTimeToServer(GetComponent<NetworkIdentity>().connectionToClient, sendTime);
	}

	[TargetRpc]
	void TargetSendTimeToServer(NetworkConnection target, float sendTime)
	{
		if(isClient)
			pingClient = Time.time-sendTime;
	}

	[TargetRpc]
	void TargetGetServerTime(NetworkConnection target, float sendTime)
	{
		if(isServer)
			pingClient = Time.time;
	}

}
