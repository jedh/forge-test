using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RPCPackager : RPCPackagerBehavior
{
	private void Awake()
	{
		GameObject.DontDestroyOnLoad(this.gameObject);
	}

	protected override void NetworkStart()
	{
		base.NetworkStart();

		GameController.Instance.AddNetworkObject(networkObject);

		if (!networkObject.IsServer && GameClient.Instance.RPCInterface == null)
		{
			GameClient.Instance.InitInterface(this);
		}
	}

	//private void Update()
	//{
	//	if (Input.GetKeyDown(KeyCode.Return))
	//	{
	//		if (networkObject.IsServer)
	//		{
	//			var bytes = Encoding.ASCII.GetBytes("FROM SERVER");
	//			networkObject.SendRpc(RPC_SEND_PACKAGE, Receivers.Others, bytes);
	//		}
	//		else
	//		{
	//			var bytes = Encoding.ASCII.GetBytes("FROM CLIENT");
	//			networkObject.SendRpc(RPC_SEND_PACKAGE, Receivers.Server, bytes);
	//		}
	//	}
	//}

	#region RPC Calls
	public void SendMessageRPC(string message)
	{
		var bytes = Encoding.ASCII.GetBytes(message);
		var receiver = Receivers.Others;
		if (!networkObject.IsServer)
		{
			receiver = Receivers.Server;
		}

		networkObject.SendRpc(RPC_SEND_PACKAGE, receiver, bytes);
	}

	public void PlayerJoinedGameRPC(in PlayerModel player)
	{
		networkObject.SendRpc(
			RPC_PLAYER_JOINED_GAME,
			Receivers.AllBuffered,
			(ushort)player.PlayerID,
			(ushort)player.TeamID,
			player.Name,
			player.NetworkID);
	}

	public void UpdatePlayerNameRPC(string newName, in PlayerModel player)
	{
		networkObject.SendRpc(
			RPC_UPDATE_PLAYER_NAME,
			Receivers.AllBuffered,
			(ushort)player.PlayerID,
			(ushort)player.TeamID,
			newName);
	}

	public void PlayerLeftGameRPC(int teamID, int playerID, uint networkID)
	{
		networkObject.SendRpc(
			RPC_PLAYER_LEFT_GAME,
			Receivers.AllBuffered,
			(ushort)playerID,
			(ushort)teamID,
			networkID);
	}

	public void PlayerChangeTeamsRPC(in PlayerModel player)
	{
		var receivers = Receivers.Server;
		if (networkObject.IsServer)
		{
			receivers = Receivers.OthersBuffered;
		}

		networkObject.SendRpc(
			RPC_PLAYER_CHANGE_TEAMS,
			receivers,
			(ushort)player.PlayerID,
			(ushort)player.TeamID);
	}
	#endregion

	#region CALLBACKS
	public override void SendPackage(RpcArgs args)
	{
		var bytes = (byte[])args.Args[0];
		var message = Encoding.ASCII.GetString(bytes);

		if (!networkObject.IsServer)
		{
			UIManager.OnClientRPCReceived?.Invoke(message);
		}
		else
		{
			UIManager.OnServerRPCReceived?.Invoke(message);
		}
	}

	public override void PlayerJoinedGame(RpcArgs args)
	{
		var playerModel = new PlayerModel()
		{
			PlayerID = (ushort)args.Args[0],
			TeamID = (ushort)args.Args[1],
			Name = (string)args.Args[2],
			NetworkID = (uint)args.Args[3]
		};

		GameClient.Instance?.PlayerJoinedGame(playerModel);
		GameServer.Instance?.PlayerJoinedGame(playerModel);
	}

	public override void UpdatePlayerName(RpcArgs args)
	{
		var playerID = (ushort)args.Args[0];
		var teamID = (ushort)args.Args[1];
		var newName = (string)args.Args[2];
		var playerGUID = args.Info.SendingPlayer.InstanceGuid;

		GameClient.Instance?.UpdatePlayerName(teamID, playerID, newName);
		GameServer.Instance?.UpdatePlayerName(playerGUID, newName);
	}

	public override void PlayerChangeTeams(RpcArgs args)
	{
		if (networkObject.IsServer)
		{
			// Server received the request to change teams.
			var playerGUID = args.Info.SendingPlayer.InstanceGuid;
			GameServer.Instance?.PlayerChangeTeams(playerGUID);
		}
		else
		{
			// Player receives confirmation that teams have changed.
			var playerID = (ushort)args.Args[0];
			var teamID = (ushort)args.Args[1];
			GameClient.Instance?.PlayerChangedTeams(teamID, playerID);
		}
	}

	public override void PlayerLeftGame(RpcArgs args)
	{
		var playerID = (ushort)args.Args[0];
		var teamID = (ushort)args.Args[1];
		var networkID = (uint)args.Args[2];

		GameClient.Instance?.PlayerLeftGame((int)teamID, (int)playerID, networkID);
		GameServer.Instance?.PlayerLeftGame((int)teamID, (int)playerID, networkID);
	}
	#endregion
}
