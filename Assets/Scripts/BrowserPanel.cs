using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrowserPanel : MonoBehaviour
{
	public GameObject BrowserItemPrefab;

	public GameObject ListContent;

	public GameObject NoMatchesText;

	public LocalServer Server;

	private void Awake()
	{
		MainThreadManager.Create();
	}

	private void Start()
	{
		NetWorker.PingForFirewall(Server.Port);
	}

	private void OnEnable()
	{
		NetWorker.localServerLocated += NetWorker_localServerLocated;
		BrowserItem.OnItemClicked += BrowserItem_OnItemClicked;
		RefreshMatches();
	}


	private void OnDisable()
	{
		NetWorker.localServerLocated -= NetWorker_localServerLocated;
		BrowserItem.OnItemClicked -= BrowserItem_OnItemClicked;
		ClearMatches();
	}

	public void Close()
	{
		this.gameObject.SetActive(false);
	}

	private void NetWorker_localServerLocated(NetWorker.BroadcastEndpoints endpoint, NetWorker sender)
	{
		MainThreadManager.Run(() =>
		{
			Debug.Log($"Found server at endpoint: {endpoint.Address}:{endpoint.Port}");
			var go = GameObject.Instantiate(BrowserItemPrefab);
			var browserItem = go.GetComponent<BrowserItem>();
			browserItem.Init(endpoint.Address, endpoint.Port);
			go.transform.SetParent(ListContent.transform);

			NoMatchesText.SetActive(false);
		});
	}

	public void RefreshMatches()
	{
		ClearMatches();
		NetWorker.RefreshLocalUdpListings(Server.Port);
	}

	public void ClearMatches()
	{
		foreach (Transform child in ListContent.transform)
		{
			GameObject.Destroy(child.gameObject);
		}

		NoMatchesText.SetActive(true);
	}

	private void BrowserItem_OnItemClicked(string ip, ushort port)
	{
		foreach (Transform listItem in ListContent.transform)
		{
			listItem.gameObject.SetActive(false);
		}

		GameObject.FindWithTag("Matchmaker")?.GetComponent<LocalServer>()?.Connect(ip, port);
	}
}
