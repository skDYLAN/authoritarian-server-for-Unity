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
    float ServerMoveVerticalOld = 0;
	bool ServerJumpOld = false;
	float ServerYRotationOld = 0;
	float ServerXRotationOld = 0; 
	//

	//Серверные
	public float maxRate = 0.03f;
    PlayerControl playerControl_server;
    //
    //параметры объекта
    [Header("Параметры RigidBody")]
    public bool useGravity = true;
    public float mass = 10f;

    protected Rigidbody _rigidbody;
    protected Collider _collider;
    protected bool _wasInit = false;
    List<ScreenTransform> screensTransformPlayerOnServer = new List<ScreenTransform>();
    uint numScreen = 0;

    void Awake()
    {
        NetworkManager_.lPlyer.Add(this);
    }

    // Use this for initialization
    void Start()
    {

        if (isServer)
        {
            StartCoroutine(IEPlayerMove());
            playerControl_server = GetComponent<PlayerControl>();

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = useGravity;
            _rigidbody.mass = mass;

        }
    }

    void Update()
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

			if(ServerJump != ServerJumpOld)
			{
				ServerJumpOld = ServerJump;
				playerControl_server.setJump(ServerJump);
			}

			if(screensTransformPlayerOnServer.Count > 0)
				if(Time.time > screensTransformPlayerOnServer[0].time + 1f)
					screensTransformPlayerOnServer.RemoveAt(0);

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
		if(isServer)
		{
			while(true)
			{
				RpcMovePlayer(transform.position, transform.rotation, Time.time);
				//screensTransformPlayerOnServer.Add(new ScreenTransform(transform.position, transform.rotation, Time.time, numScreen));
				numScreen++;
				yield return new WaitForSeconds(maxRate);
			}
		}
	}


}
