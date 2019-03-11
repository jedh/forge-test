using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientInterface
{
	private RPCPackager m_RPCPackager;

	public ClientInterface(RPCPackager rpcPackager)
	{
		m_RPCPackager = rpcPackager;
	}

	public void PlayerChangeTeams(in PlayerModel player)
	{
		m_RPCPackager.PlayerChangeTeamsRPC(player);
	}

	public void UpdatePlayerName(string newName, PlayerModel player)
	{
		m_RPCPackager.UpdatePlayerNameRPC(newName, player);
	}
}
