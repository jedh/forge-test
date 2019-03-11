using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Lobby;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameController;

public class GameServer : MonoBehaviour
{
	public static GameServer Instance { get; private set; }

	public ServerInterface RPCInterface { get; private set; }

	public int MaxPlayers;

	private UDPServer m_server;

	public Dictionary<string, PlayerModel> Players { get; private set; }

	public void Init(NetWorker server, ServerConfig config, RPCPackager rpcPackager)
	{
		Instance = this;
		m_server = (UDPServer)server;
		Players = new Dictionary<string, PlayerModel>();
		MaxPlayers = config.MaxPlayers;
		RPCInterface = new ServerInterface(rpcPackager);

		m_server.playerTimeout += (player, sender) =>
		{
			Debug.Log($"Player: {player.NetworkId} timed out.");
		};

		m_server.playerConnected += (player, sender) =>
		{
			PlayerConnected(player);
		};

		m_server.playerGuidAssigned += (player, sender) =>
		{
			Debug.Log($"Player: {player.NetworkId} assigned GUID: {player.InstanceGuid}");
		};

		m_server.playerAccepted += (player, sender) =>
		{
			MainThreadManager.Run(() => { PlayerAccepted(player); });
		};

		m_server.playerRejected += (player, sender) =>
		{
			Debug.Log($"Player: {player.NetworkId} rejected.");
		};

		m_server.playerDisconnected += (player, sender) =>
		{
			PlayerDisconnected(player);
		};
	}

	private void PlayerConnected(NetworkingPlayer player)
	{
		Debug.Log($"Player: {player.NetworkId} connected.");

		if (m_server.udpPlayers.Count >= MaxPlayers)
		{
			m_server.Disconnect(player, true);
		}
	}

	private void PlayerAccepted(NetworkingPlayer player)
	{
		Debug.Log($"Player: {player.InstanceGuid} accepted.");

		if (Players.ContainsKey(player.InstanceGuid)) { return; }
		var teamID = 0;
		if (Players.Count % 2 > 0)
		{
			teamID = 1;
		}

		var playerModel = new PlayerModel()
		{
			ID = player.InstanceGuid,
			PlayerID = (int)player.NetworkId,
			TeamID = teamID,
			Name = $"player{(int)player.NetworkId}",
			NetworkID = player.NetworkId
		};

		Players.Add(playerModel.ID, playerModel);
		RPCInterface.PlayerJoinedGame(playerModel);
	}

	private void PlayerDisconnected(NetworkingPlayer player)
	{
		Debug.Log($"Player: {player.NetworkId} disconnected.");

		if (Players.ContainsKey(player.InstanceGuid))
		{
			var playerModel = Players[player.InstanceGuid];
			Players.Remove(player.InstanceGuid);
			RPCInterface.PlayerLeftGame(playerModel.TeamID, playerModel.PlayerID, playerModel.NetworkID);
		}
	}

	public void PlayerJoinedGame(PlayerModel playerModel)
	{
		SharedEvents.OnPlayerJoinedGame?.Invoke(playerModel);
	}

	public void PlayerLeftGame(int teamID, int playerID, uint networkID)
	{
		SharedEvents.OnPlayerLeftGame?.Invoke(teamID, playerID, networkID);
	}

	public void PlayerChangeTeams(string playerGUID)
	{
		if (Players.ContainsKey(playerGUID))
		{
			var player = Players[playerGUID];
			var newTeamID = 0;
			if (player.TeamID == 0)
			{
				newTeamID = 1;
			}

			player.TeamID = newTeamID;
			RPCInterface.PlayerChangeTeams(player);

			SharedEvents.OnPlayerChangedTeam?.Invoke(player);
		}
	}

	public void UpdatePlayerName(string playerGUID, string newName)
	{
		if (Players.ContainsKey(playerGUID))
		{
			var player = Players[playerGUID];
			player.Name = newName;
			SharedEvents.OnUpdatePlayerName?.Invoke(player);
		}
	}
}
