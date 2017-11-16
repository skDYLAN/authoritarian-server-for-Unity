using UnityEngine;

public class ScreenTransform {

	public Vector3 position;
	public Quaternion rotation;
	public float time;

	public ScreenTransform(Vector3 _position, Quaternion _rotation, float _time)
	{
		position = _position;
		rotation = _rotation;
		time = _time;
	}
}
