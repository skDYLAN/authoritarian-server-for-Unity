using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerControl_Client : NetworkBehaviour {

	public bool masterClient = false;
    public NetGM_Local netGM_Local;
    public List<ScreenTransform> _scrinsTransformPlayer = new List<ScreenTransform>();
    public GameObject objNetGM_Local;
    public PlayerControl_LocalClient playerControl_LocalClient;


    public int tick=0;
	public int tickDel=0;
	bool flagJourney = false;
    bool localPlayer = false;
	
	Rigidbody _rigidbody;

    Vector3 endPosition;
    Quaternion endRotation;
    Vector3 startMarker_Position;
    Quaternion startMarker_Rotation;

    public int Count;
    float prec=0;

	float startTime;
    float restOfTime=0;

	uint numScreen=0;
	int extr = 1;
    uint numScr = 0;
    float timer;

	void Start()
	{
        if(isServer)
            return;

        startTime = Time.time;

    }
	void Update()
	{
		if(isServer)
            return;

        if(objNetGM_Local == null)
            objNetGM_Local = GameObject.Find("GM_local_main");

        if(objNetGM_Local != null && netGM_Local == null)
            netGM_Local = objNetGM_Local.GetComponent<NetGM_Local>();

        if(objNetGM_Local != null && playerControl_LocalClient == null)
            playerControl_LocalClient = objNetGM_Local.GetComponent<PlayerControl_LocalClient>();


        if (masterClient == false && netGM_Local != null)
		{	
			tick++;
			Count = _scrinsTransformPlayer.Count;

            
              if(netGM_Local.GetServerTime() - 0.1f > (timer + 0.1f))
                  flagJourney = true;

            /*
           if(Count>0)
               if(_scrinsTransformPlayer[0].time > timer + 0.19f && _scrinsTransformPlayer[0].time < timer+0.11f)
                   flagJourney = true;
            */
            // Эстраполяция

            if (flagJourney == true && extr == 0)
			{   

				    endPosition = (endPosition - startMarker_Position) + endPosition;

                
                startMarker_Position = transform.position;
                numScr++;

                
                if (!localPlayer)
                {
                    endRotation = Quaternion.Slerp(startMarker_Rotation, endRotation, 2);
                    startMarker_Rotation = transform.rotation;
                }


                tickDel++;

                startTime = Time.time;
                extr = 1;
                flagJourney = false;
			}
 
            if (_scrinsTransformPlayer.Count > 0)
			{
                if (netGM_Local.GetServerTime() - 0.1f >= _scrinsTransformPlayer[0].time)
                {
                    startMarker_Position = transform.position;
                    endPosition = _scrinsTransformPlayer[0].position;

                    if(!localPlayer)
                    {
                        startMarker_Rotation = transform.rotation;
                        endRotation = _scrinsTransformPlayer[0].rotation;
                    }



                    startTime = Time.time;

                    numScr = _scrinsTransformPlayer[0].number;
                    timer = _scrinsTransformPlayer[0].time;
                    _scrinsTransformPlayer.RemoveAt(0);

                    extr = 0;
                }
                
            }

                prec = ((Time.time) - startTime) / (0.05f - restOfTime);
            /*
            if(prec > 1.2 && extr == 0)
            {
                extr = 1;
                numScr++;
                tickDel++;
            }
            /*
            if (true)
            {
                if (prec > 1)
                {
                     restOfTime = (Time.time - startTime) - (0.05f - restOfTime);
                     restOfTime = 0;
                     //prec = 1;

                }
                else
                {
                    restOfTime = 0;
                }
            }

            */

            if (!localPlayer)
            {
                transform.position = Vector3.Lerp(startMarker_Position, endPosition, prec);
                transform.rotation = Quaternion.Slerp(startMarker_Rotation, endRotation, prec);

            }
            else
                if (playerControl_LocalClient.moveV == false && playerControl_LocalClient.moveH == false)
                {
                    transform.position = Vector3.Lerp(startMarker_Position, endPosition, prec);
                }

        }
	
	}

	[ClientCallback]
	public void SetNewPositionOfPlayer(Vector3 newPosition, Quaternion newRotation, float timeScreen)
	{
		_scrinsTransformPlayer.Add(new ScreenTransform(newPosition, newRotation, timeScreen, numScreen));
		numScreen++;
		
	}

    public void SetObjGM(GameObject obj)
    {
        objNetGM_Local = obj;
        netGM_Local = obj.GetComponent<NetGM_Local>();
    }
    public void SetLolcalPlayer(bool status)
    {
        localPlayer = status;
    }

}
