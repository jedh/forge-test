using BeardedManStudios.Forge.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
	public LocalServer Server;
	public GameObject HostPanel;
	public GameObject BrowserPanel;
	public InputField PortInputField;
	private void Start()
	{
		var go = GameObject.FindWithTag("Matchmaker");
		Server = go?.GetComponent<LocalServer>();
	}

	private void OnEnable()
	{
		PortInputField.text = Server.Port.ToString();
	}

	public void HostSelected()
	{
		NetWorker.PingForFirewall(Server.Port);
		BrowserPanel.SetActive(false);
		HostPanel.SetActive(true);
	}

	public void BrowseSelected()
	{
		NetWorker.PingForFirewall(Server.Port);
		HostPanel.SetActive(false);
		BrowserPanel.SetActive(true);
	}

	public void PortInputUpdated()
	{
		Server.Port = ushort.Parse(PortInputField.text);
	}
}
