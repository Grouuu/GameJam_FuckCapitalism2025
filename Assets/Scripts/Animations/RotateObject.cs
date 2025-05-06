using UnityEngine;

public class RotateObject : MonoBehaviour
{
	public Vector3 rotations = Vector3.zero;

	private void Update ()
	{
		transform.Rotate(rotations * Time.deltaTime);
	}

}
