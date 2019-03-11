using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LobbyPlayerPanelItem : MonoBehaviour
{
	public PlayerModel Player { get; private set; }

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SetPlayer(in PlayerModel playerModel)
	{
		Player = playerModel;
		this.GetComponent<TextMeshProUGUI>().SetText(playerModel.Name);
	}

	public void SetPlayerName(string playerName)
	{
		this.GetComponent<TextMeshProUGUI>().SetText(playerName);
	}
}
