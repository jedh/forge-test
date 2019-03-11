using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
	public static Action<string> OnClientRPCReceived;
	public static Action<string> OnServerRPCReceived;

	public TextMeshProUGUI Text;

	private void OnEnable()
	{
		OnClientRPCReceived += ClientRPCReceived;
		OnServerRPCReceived += ServerRPCReceived;
	}

	private void OnDisable()
	{
		OnClientRPCReceived -= ClientRPCReceived;
		OnServerRPCReceived -= ServerRPCReceived;
	}

	private void ClientRPCReceived(string message)
	{
		Text?.SetText($"Client received message: {message}");
	}

	private void ServerRPCReceived(string message)
	{
		Text?.SetText($"Server received message: {message}");
	}
}
