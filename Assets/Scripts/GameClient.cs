using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClient : MonoBehaviour
{
	public static GameClient Instance { get; private set; }

	public ClientInterface RPCInterface { get; private set; }

	private UDPClient m_Client;

	public uint NetworkID { get; private set; }

	public int PlayerID { get; private set; }

	public int TeamID { get; private set; }

	public PlayerModel Player
	{
		get
		{
			if (Players != null && Players.ContainsKey((TeamID, PlayerID)))
			{
				return Players[(TeamID, PlayerID)];
			}
			else
			{
				return null;
			}
		}
	}

	// (int, int) = (team id, player id)
	public Dictionary<(int, int), PlayerModel> Players { get; private set; }

	public void Init(NetWorker client)
	{
		Instance = this;
		m_Client = (UDPClient)client;
		Players = new Dictionary<(int, int), PlayerModel>();

		m_Client.serverAccepted += (sender) =>
		{
			ServerAccepted(sender);
		};

		m_Client.disconnected += (sender) =>
		{
			Debug.Log("Disconnected.");
			MainThreadManager.Run(() =>
			{
				//NetWorker.EndSession();
				NetworkManager.Instance.Networker.IterateNetworkObjects((obj) =>
				{
					obj.Destroy(100);
				});

				//GameController.Instance.DestroyNetworkObjects();
				SceneManager.LoadScene(0);
			});
		};

		m_Client.forcedDisconnect += (sender) =>
		{
			Debug.Log("Force disconnected.");
		};
	}

	public void InitInterface(RPCPackager rpcPackager)
	{
		RPCInterface = new ClientInterface(rpcPackager);
	}

	private void ServerAccepted(NetWorker sender)
	{
		Debug.Log("Server accepted player");

		NetworkID = sender.Me.NetworkId;

		// Send RPC to update the player name.

		// Send RPC to request players in the match.
	}

	public void PlayerJoinedGame(PlayerModel playerModel)
	{
		if (!Players.ContainsKey((playerModel.TeamID, playerModel.PlayerID)))
		{
			Players.Add((playerModel.TeamID, playerModel.PlayerID), playerModel);
		}

		if (playerModel.NetworkID == NetworkID)
		{
			TeamID = playerModel.TeamID;
			PlayerID = playerModel.PlayerID;
		}

		SharedEvents.OnPlayerJoinedGame?.Invoke(playerModel);
	}

	public void PlayerLeftGame(int teamID, int playerID, uint networkID)
	{
		// Remove players from collection.
		if (Players.ContainsKey((teamID, playerID)))
		{
			Players.Remove((teamID, playerID));
		}

		SharedEvents.OnPlayerLeftGame?.Invoke(teamID, playerID, networkID);
	}

	public void PlayerChangedTeams(int newTeamID, int playerID)
	{
		var prevTeamID = newTeamID > 0 ? 0 : 1;

		if (Players.ContainsKey((prevTeamID, playerID)))
		{
			var player = Players[(prevTeamID, playerID)];
			Players.Remove((prevTeamID, playerID));
			Players.Add((newTeamID, playerID), player);
			player.TeamID = newTeamID;

			if (NetworkID == player.NetworkID)
			{
				TeamID = newTeamID;
			}

			SharedEvents.OnPlayerChangedTeam?.Invoke(player);
		}
	}

	public void UpdatePlayerName(int teamID, int playerID, string newName)
	{
		if (Players.ContainsKey((teamID, playerID)))
		{
			var player = Players[(teamID, playerID)];
			player.Name = newName;
			SharedEvents.OnUpdatePlayerName?.Invoke(player);
		}
	}
}
