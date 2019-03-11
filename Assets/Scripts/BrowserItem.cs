using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrowserItem : MonoBehaviour
{
	public delegate void ItemClicked(string ip, ushort port);
	public static event ItemClicked OnItemClicked;

	public Text TextField;

	[HideInInspector]
	public string Name;

	[HideInInspector]
	public string IPAddress;

	[HideInInspector]
	public ushort Port;

	public void OnSelected()
	{
		Debug.Log($"Try to connect to: {TextField.text}");
		this.GetComponent<Button>().interactable = false;
		OnItemClicked?.Invoke(IPAddress, Port);
	}

	public void Init(string ipAddress, ushort port, string name = "")
	{
		IPAddress = ipAddress;
		Port = port;
		Name = name;

		if (!string.IsNullOrEmpty(Name))
		{
			TextField.text = Name;
		}
		else
		{
			TextField.text = $"{IPAddress}:{Port}";
		}
	}
}
