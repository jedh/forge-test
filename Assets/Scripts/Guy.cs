using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using UnityEngine;

public class Guy : GuyBehavior
{
	private void Update()
	{
		if (networkObject.IsServer)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				networkObject.SendRpc(RPC_MOVE_UP, Receivers.Others);
			}
		}
	}

	public override void MoveUp(RpcArgs args)
	{
		transform.position += Vector3.up;
	}
}
