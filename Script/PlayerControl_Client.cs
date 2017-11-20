using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl_Client : NetworkBehaviour {

	public Vector3 newPositionOfPlayer;
	public float _delta = 0;
	public bool masterClient = false;

	public List<ScreenTransform> _scrinsTransformPlayer = new List<ScreenTransform>();

	public float speed = 1;
	float t =0;

	public int tick=0;
	public int tickDel=0;
	bool flagJourney = false;
	
	Rigidbody _rigidbody;
	float EndTime = 0;

    Vector3 endPosition;
    Quaternion endRotation;
    Vector3 startMarker_Position;
    Quaternion startMarker_Rotation;

    public int Count;
    public float deltaResult = 0;
    float prec=0;

    Vector3 newPos;
	float distCur;
	float startTime;
	float StarttimeWay;
    float restOfTime=0;

	uint numScreen=1;
	int extr = 1;
    uint numScr = 0;
    public float coefInExtr=1;

    int step = 0;


	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
        if (isClient)
            startTime = Time.time;

    }
	[Client]
	void Update()
	{
		
		if(masterClient == false)
		{	
			tick++;
			Count = _scrinsTransformPlayer.Count;

            if (extr == 1 && _scrinsTransformPlayer.Count !=0)
                if(numScr == _scrinsTransformPlayer[0].number)
                    _scrinsTransformPlayer.RemoveAt(0);

            // Эстраполяция
            if (_scrinsTransformPlayer.Count == 0 && prec > coefInExtr && extr == 0)
			{
                
				endPosition = (endPosition - startMarker_Position) + endPosition;
                startMarker_Position = transform.position;
                numScr++;

                /*
                if (!isLocalPlayer)
                {
                    startMarker_Rotation = transform.rotation;
                    endRotation = _scrinsTransformPlayer[0].rotation;
                }
                */

                tickDel++;

                startTime = Time.time ;
                flagJourney = true;
                extr = 1;
			}


            if (_scrinsTransformPlayer.Count > 0)
			{
                startMarker_Position = transform.position;  
				endPosition = _scrinsTransformPlayer[0].position;

                if(!isLocalPlayer)
                {
                    startMarker_Rotation = transform.rotation;
                    endRotation = _scrinsTransformPlayer[0].rotation;
                }



                startTime = Time.time;
                flagJourney = true;
                numScr= _scrinsTransformPlayer[0].number;
                _scrinsTransformPlayer.RemoveAt(0);
				extr = 0;
                
            }

                prec = ((Time.time) - startTime) / (0.1f);
                if (prec >= 1)
                {
                    flagJourney = false;
                    if (prec > 1)
                    {
                         restOfTime = ((Time.time) - startTime) - (0.1f);
                         prec = 1;
                    }
                    else
                    {
                        restOfTime = 0;
                    }

                    Debug.Log(restOfTime);
                }

                transform.position = Vector3.Lerp(startMarker_Position, endPosition, prec);
                Debug.Log(string.Format("Pos{0} -- End{1}", transform.position, endPosition));
                if (!isLocalPlayer)
                    transform.rotation = Quaternion.Slerp(startMarker_Rotation, endRotation, prec);

        }
		
		
		

	}

	[ClientCallback]
	public void SetNewPositionOfPlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		_scrinsTransformPlayer.Add(new ScreenTransform(newPosition, newRotation, timeScreen, numScreen));
        if (numScreen > 1)
        {
            _delta += timeScreen - t;
            deltaResult = (_delta / (numScreen + 1))-0.05f;
        }
		t = timeScreen;
		numScreen++;
		
	}

	void CheckScreens()
	{
		
	}

}
