using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    [SerializeField] float force = 0f;

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody>().AddForce(transform.forward * force);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer)
        {
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("попал");
                collision.gameObject.GetComponent<PlayerNetwork>().GetHit(10f);
            }
             NetworkServer.Destroy(gameObject);
        }
    }
}
