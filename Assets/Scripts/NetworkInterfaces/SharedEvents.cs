using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedEvents
{
	public static Action<PlayerModel> OnPlayerJoinedGame;

	/// <summary>
	/// int - team id
	/// int - player id
	/// uint - network id
	/// </summary>
	public static Action<int, int, uint> OnPlayerLeftGame;

	public static Action<PlayerModel> OnPlayerChangedTeam;

	public static Action<PlayerModel> OnUpdatePlayerName;
}
