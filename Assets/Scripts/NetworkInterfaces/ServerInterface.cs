using BeardedManStudios.Forge.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInterface
{
	//public static ServerInterface Instance { get; private set; }

	private RPCPackager m_RPCPackager;

	public ServerInterface(RPCPackager rpcPackager)
	{
		m_RPCPackager = rpcPackager;
	}

	public void PlayerJoinedGame(in PlayerModel player)
	{
		m_RPCPackager.PlayerJoinedGameRPC(player);
	}

	public void PlayerLeftGame(int teamID, int playerID, uint networkID)
	{
		m_RPCPackager.PlayerLeftGameRPC(teamID, playerID, networkID);
	}

	public void PlayerChangeTeams(in PlayerModel player)
	{
		m_RPCPackager.PlayerChangeTeamsRPC(player);
	}
}
