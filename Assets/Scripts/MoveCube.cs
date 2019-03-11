using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCube : MoveCubeBehavior
{
	private void Update()
	{
		if (!networkObject.IsServer)
		{
			transform.position = networkObject.position;
			transform.rotation = networkObject.rotation;
			return;
		}

		transform.position += new Vector3(
			Input.GetAxis("Horizontal"),
			Input.GetAxis("Vertical"),
			0f
		) * Time.deltaTime * 5f;

		transform.Rotate(Vector3.up, Time.deltaTime * 90.0f);

		networkObject.position = transform.position;
		networkObject.rotation = transform.rotation;
	}
}
