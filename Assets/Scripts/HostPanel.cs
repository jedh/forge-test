using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostPanel : MonoBehaviour
{
	public InputField IPInput;
	public InputField PortInput;
	public InputField MatchNameInput;
	public Button StartHostingButton;

	public LocalServer Server;

	private void Start()
	{
		IPInput.text = Server.IPAddress;
		PortInput.text = Server.Port.ToString();
	}

	public void Close()
	{
		this.gameObject.SetActive(false);
	}

	public void StartHostingSelected()
	{
		if (!string.IsNullOrEmpty(IPInput.text) &&
			!string.IsNullOrEmpty(PortInput.text))
		{
			Server.Host(IPInput.text, ushort.Parse(PortInput.text), MatchNameInput.text);
			StartHostingButton.enabled = false;
		}
	}
}
