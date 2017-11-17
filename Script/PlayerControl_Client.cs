using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControl_Client : NetworkBehaviour {

	public Vector3 newPositionOfPlayer;
	public float _delta;
	public bool masterClient = false;

	List<ScreenTransform> _scrinsTransformPlayer = new List<ScreenTransform>();

	public float speed = 1;
	float t =0;

	public int tick=0;
	 int tickDel=0;
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
			if(_scrinsTransformPlayer.Count > 1 && fracJourney >=1) //&& fracJourney >=1
			{
				_scrinsTransformPlayer.RemoveAt(0);
				startMarker = transform;
				endPosition = _scrinsTransformPlayer[0].position;
				journeyLength = Vector3.Distance(startMarker.position, endPosition);
				if(journeyLength != 0.0f)
				{
					EndTime = Time.time + 0.1f;
					distCovered = 0f;
					fracJourney=0f;
					speed = journeyLength/0.1f;
					distCur=0f;
					// Замерять сколько прошел
				}
				_scrinsTransformPlayer.RemoveAt(0);
				tickDel++;
			}

			if(fracJourney < 1)
			{
				distCovered = journeyLength - ((EndTime - Time.time) * speed - speed);
				fracJourney = distCovered / journeyLength;
				transform.position = Vector3.Lerp(startMarker.position, endPosition, fracJourney);
				distCur = Vector3.Distance(transform.position, endPosition);
			}
 		
		}
		
		
		

	}

	[Client]
	public void SetNewPositionOfPlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		_scrinsTransformPlayer.Add(new ScreenTransform(newPosition, newRotation, timeScreen));
		//	_scrinsTransformPlayer.Add(screenTransform);
	}

	void CheckScreens()
	{
		
	}

}
