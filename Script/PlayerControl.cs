using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour {

    float _moveHorizontal = 0;
    float _moveVertical = 0;
	bool _jump = false;

	float _yRotation = 0;
	float _xRotation = 0; 
	float _yRotationOld = 0;
	float _xRotationOld = 0; 

	[SerializeField] float speed = 1f;

	[SerializeField] float lookSpeed = 1;

	[SerializeField] float forceJump = 1;
	Rigidbody _rigidbody;

	Vector3 rotationResult;

	public Vector3 test;

    // 
	// Use this for initialization
	void Start () {
		_rigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	
	void FixedUpdate()
	{
		// управление
		Vector3 velocity = (transform.right * _moveHorizontal + transform.forward*_moveVertical).normalized * speed;
		test = velocity;
		
		_rigidbody.MovePosition(_rigidbody.position + velocity * Time.deltaTime);

		if(_yRotation != _yRotationOld || _xRotation != _xRotationOld)
		{
			Vector3 rotation = new Vector3(-_yRotation, _xRotation, 0f) * lookSpeed;

			if(Mathf.Sin(rotation.x * Mathf.Deg2Rad) < 0f && Mathf.Sin(rotation.x * Mathf.Deg2Rad) > -0.5f
				|| Mathf.Sin(rotation.x * Mathf.Deg2Rad) > 0f && Mathf.Sin(rotation.x * Mathf.Deg2Rad) < 0.5f)
					rotationResult = rotation;
			transform.eulerAngles = new Vector3(rotationResult.x, rotation.y,0f);

			_yRotationOld = _yRotation;
			_xRotationOld = _xRotation;
		}
		if(_jump == true)
			_rigidbody.AddForce(transform.up * forceJump); // сделать возможность только одного прыжка
		
		// конец управления

	}

	public void playerRotationSlerp(Quaternion toRotation)
	{
		transform.rotation = toRotation;
	}
	public void playerPosition(Vector3 newPosition)
	{
		//newPositionPlayer = newPosition;
		//_rigidbody.MovePosition(newPosition);
	}
	public void setMoveHorizontal(float moveHorizontal)
	{
		_moveHorizontal = moveHorizontal;
	}

	public void setMoveVertical(float moveVertical)
	{
		_moveVertical = moveVertical;
	}
	public void setRotationX(float xRotation)
	{
		_xRotation = xRotation;
	}
	public void setRotationY(float yRotation)
	{
		_yRotation = yRotation;
	}
	public void setJump(bool jump)
	{
		_jump = jump;
	}
}
