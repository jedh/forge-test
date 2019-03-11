using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	public GameObject LeftTeamPanel;

	public GameObject RightTeamPanel;

	public GameObject PlayerPanelItem;

	public Button SwitchSidesButton;

	public InputField NameInput;

	public Button UpdateNameButton;

	public GameObject NamePanel;

	private void Start()
	{
		UpdateNameButton.interactable = false;

		if (GameController.Instance.IsServer)
		{
			SwitchSidesButton.gameObject.SetActive(false);
			NamePanel.SetActive(false);
		}

		SharedEvents.OnPlayerLeftGame += (teamID, playerID, networkID) =>
		{
			MainThreadManager.Run(() => { OnPlayerLeftGame(teamID, playerID, networkID); });
		};

		SharedEvents.OnPlayerChangedTeam += (playerModel) =>
		{
			MainThreadManager.Run(() => { OnPlayerChangedTeam(playerModel); });
		};

		SharedEvents.OnUpdatePlayerName += (playerModel) =>
		{
			MainThreadManager.Run(() => { OnUpdatePlayerName(playerModel); });
		};
	}

	private void OnEnable()
	{
		SharedEvents.OnPlayerJoinedGame += OnPlayerJoinedGame;
		//SharedEvents.OnPlayerLeftGame += OnPlayerLeftGame;
	}

	private void OnDisable()
	{
		SharedEvents.OnPlayerJoinedGame -= OnPlayerJoinedGame;
		//SharedEvents.OnPlayerLeftGame -= OnPlayerLeftGame;
	}

	public void SwitchSidesSelected()
	{

		SwitchSidesButton.interactable = false;

		GameClient.Instance.RPCInterface.PlayerChangeTeams(GameClient.Instance.Player);
	}

	public void NameInputUpdated()
	{
		if (!string.IsNullOrEmpty(NameInput.text) && NameInput.text != GameClient.Instance.Player.Name)
		{
			UpdateNameButton.interactable = true;
		}
		else
		{
			UpdateNameButton.interactable = false;
		}
	}

	public void UpdateNameSelected()
	{
		var updatedName = NameInput.text;
		if (IsNameUnique(updatedName, LeftTeamPanel) && IsNameUnique(updatedName, RightTeamPanel))
		{
			UpdateNameButton.interactable = false;
			NameInput.interactable = false;
			GameClient.Instance?.RPCInterface.UpdatePlayerName(updatedName, GameClient.Instance.Player);
		}
	}

	private void OnPlayerJoinedGame(PlayerModel playerModel)
	{
		var playerGO = GameObject.Instantiate(PlayerPanelItem);
		playerGO.GetComponent<LobbyPlayerPanelItem>().SetPlayer(playerModel);

		if (playerModel.TeamID == 0)
		{
			playerGO.transform.SetParent(LeftTeamPanel.transform);
		}
		else
		{
			playerGO.transform.SetParent(RightTeamPanel.transform);
		}

		// Sort lists.		
		OrderPlayersInPanel(LeftTeamPanel);
		OrderPlayersInPanel(RightTeamPanel);

		if (GameClient.Instance != null)
		{
			NameInput.text = GameClient.Instance.Player.Name;
		}
	}

	private void OnPlayerLeftGame(int teamID, int playerID, uint networkID)
	{
		var playerItem = GetPlayerItemFromPanel(LeftTeamPanel, networkID);
		if (playerItem == null)
		{
			playerItem = GetPlayerItemFromPanel(RightTeamPanel, networkID);
		}

		if (playerItem != null)
		{
			Destroy(playerItem.gameObject);
		}
	}

	private void OnPlayerChangedTeam(PlayerModel player)
	{
		var playerItem = GetPlayerItemFromPanel(LeftTeamPanel, player.NetworkID);
		if (playerItem != null)
		{
			playerItem.transform.SetParent(RightTeamPanel.transform);
		}
		else
		{
			playerItem = GetPlayerItemFromPanel(RightTeamPanel, player.NetworkID);
			playerItem?.transform.SetParent(LeftTeamPanel.transform);
		}

		if (playerItem != null)
		{
			OrderPlayersInPanel(LeftTeamPanel);
			OrderPlayersInPanel(RightTeamPanel);
		}

		if (SwitchSidesButton.interactable == false && GameClient.Instance != null)
		{
			if (GameClient.Instance.Player.NetworkID == player.NetworkID)
			{
				SwitchSidesButton.interactable = true;
			}
		}
	}

	private void OnUpdatePlayerName(PlayerModel player)
	{
		var playerItem = GetPlayerItemInLobby(player.NetworkID);
		if (playerItem != null)
		{
			playerItem.SetPlayerName(player.Name);
			if (player.NetworkID == GameClient.Instance?.NetworkID && !NameInput.interactable)
			{
				NameInput.interactable = true;
				//UpdateNameButton.interactable = true;
			};
		}
	}

	private LobbyPlayerPanelItem GetPlayerItemInLobby(uint networkID)
	{
		var playerItem = GetPlayerItemFromPanel(LeftTeamPanel, networkID);
		if (playerItem == null)
		{
			playerItem = GetPlayerItemFromPanel(RightTeamPanel, networkID);
		}

		return playerItem;
	}

	private LobbyPlayerPanelItem GetPlayerItemFromPanel(GameObject panel, uint networkID)
	{
		if (panel == null) { return null; }

		LobbyPlayerPanelItem playerItem = null;
		foreach (Transform trans in panel.transform)
		{
			var panelItem = trans.GetComponent<LobbyPlayerPanelItem>();
			if (panelItem != null)
			{
				if (panelItem.Player.NetworkID == networkID)
				{
					playerItem = panelItem;
					break;
				}
			}
		}

		return playerItem;
	}

	private void OrderPlayersInPanel(GameObject panel)
	{
		var players = new List<LobbyPlayerPanelItem>();
		foreach (Transform trans in panel.transform)
		{
			var playerItem = trans.GetComponent<LobbyPlayerPanelItem>();
			if (playerItem != null)
			{
				players.Add(playerItem);
			}
		}

		players = players.OrderBy(o => o.Player.NetworkID).ToList();

		for (int i = 0; i < players.Count; i++)
		{
			players[i].transform.SetSiblingIndex(i);
		}

		//LayoutRebuilder.ForceRebuildLayoutImmediate(panel.GetComponent<RectTransform>());
	}

	private bool IsNameUnique(string playerName, GameObject teamPanel)
	{
		bool isUnique = true;
		foreach (Transform trans in teamPanel.transform)
		{
			var panelItem = trans.GetComponent<LobbyPlayerPanelItem>();
			if (panelItem != null && panelItem.Player.Name == playerName)
			{
				isUnique = false;
				break;
			}
		}

		return isUnique;
	}
}
