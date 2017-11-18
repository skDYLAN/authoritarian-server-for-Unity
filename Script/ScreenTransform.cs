using UnityEngine;

public class ScreenTransform {

	public Vector3 position;
	public Quaternion rotation;
	public float time;
	public uint number;

	public ScreenTransform(Vector3 _position, Quaternion _rotation, float _time, uint _number)
	{
		position = _position;
		rotation = _rotation;
		time = _time;
		number = _number;
		
	}
}
