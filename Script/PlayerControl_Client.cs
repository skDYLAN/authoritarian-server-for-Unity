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
	 float fracJourney = 1f;
	Vector3 endPosition;
	Rigidbody _rigidbody;
	 float journeyLength = 0;
	 float distCovered = 0;
	 float EndTime = 0;
	Transform startMarker;
	public int Count;
	 Vector3 newPos;
	float distCur;
	float timeWay;
	float StarttimeWay;

	uint numScreen=1;
	int extr = 0;

    int step = 0;


	void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}
	[Client]
	void Update()
	{
		
		if(masterClient == false)
		{	
			tick++;
			Count = _scrinsTransformPlayer.Count;
			
			// Эстраполяция
			if(_scrinsTransformPlayer.Count == 1 && fracJourney >=1 && extr == 0)
			{
                Debug.Log("extr");
				startMarker = transform;
				endPosition = (_scrinsTransformPlayer[0].position - startMarker.position) + _scrinsTransformPlayer[0].position;
				journeyLength = Vector3.Distance(startMarker.position, endPosition);
				if(journeyLength != 0.0f)
				{
					EndTime = Time.time + 0.1f;
					distCovered = 0f;
					fracJourney=0f;
					speed = journeyLength/1f;
					distCur=0f;
					StarttimeWay  = Time.time;
					// Замерять сколько прошел
				}
                Debug.Log(_scrinsTransformPlayer[0].number);
				_scrinsTransformPlayer.RemoveAt(0);
				tickDel++;
				extr = 1;
			}


            if (_scrinsTransformPlayer.Count > 1 + extr && fracJourney >=1)
			{
                if(extr==1)
                    _scrinsTransformPlayer.RemoveAt(0);
                _scrinsTransformPlayer.RemoveAt(0);
				startMarker = transform;
				endPosition = _scrinsTransformPlayer[0].position;
                Debug.Log(_scrinsTransformPlayer[0].number);
				journeyLength = Vector3.Distance(startMarker.position, endPosition);
				if(journeyLength != 0.0f)
				{
					EndTime = Time.time + 0.1f;
					distCovered = 0f;
					fracJourney=0f;
					speed = journeyLength/ 1f;
					distCur=0f;
					StarttimeWay  = Time.time;
					// Замерять сколько прошел
				}
				_scrinsTransformPlayer.RemoveAt(0);
				extr = 0;
			}

			if(fracJourney < 1)
			{
                float t = EndTime - Time.time;
                if (t < 0)
                {
                    fracJourney = 1;
                }
                else
                {
                    distCovered = journeyLength - ((t) * speed);
                    fracJourney = distCovered / journeyLength;
                }
				transform.position = Vector3.Lerp(startMarker.position, endPosition, Time.time/100);
				distCur = Vector3.Distance(transform.position, endPosition);

				timeWay = Time.time;
                step++;
                
            }


        }
		
		
		

	}

	[ClientCallback]
	public void SetNewPositionOfPlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		_scrinsTransformPlayer.Add(new ScreenTransform(newPosition, newRotation, timeScreen, numScreen));
		_delta += timeScreen - t;
		//Debug.Log(_delta/numScreen);
		t = timeScreen;
		numScreen++;
		
	}

	void CheckScreens()
	{
		
	}

}
