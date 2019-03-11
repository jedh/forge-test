using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
	public static GameController Instance { get; private set; }

	public GameServer Server { get; private set; }
	public bool IsServer { get { return Server != null; } }

	public GameClient Client { get; private set; }

	[Serializable]
	public struct ServerConfig
	{
		[Range(0, 100)]
		public int MaxPlayers;
	}

	public ServerConfig ServerSettings;

	public List<NetworkObject> NetworkObjects { get; private set; }

	private void Awake()
	{
		GameObject.DontDestroyOnLoad(this.gameObject);

		if (Instance == null)
		{
			Instance = this;
		}

		NetworkObjects = new List<NetworkObject>();
	}

	private void Start()
	{
		//var rpcPackager = GameObject.FindWithTag("RPCPackager")?.GetComponent<RPCPackager>();
		//var rpcPackager = this.gameObject.AddComponent<RPCPackager>();		
	}

	public void AttachServer(NetWorker server, RPCPackagerBehavior rpcPackagerBehavior)
	{
		var gameServer = this.gameObject.AddComponent<GameServer>();
		gameServer.Init(server, ServerSettings, rpcPackagerBehavior.GetComponent<RPCPackager>());
		Server = gameServer;
	}

	public void AttachClient(NetWorker client)
	{
		var gameClient = this.gameObject.AddComponent<GameClient>();
		gameClient.Init(client);
		Client = gameClient;
	}

	public void AddNetworkObject(NetworkObject networkObject)
	{
		NetworkObjects.Add(networkObject);
	}

	public void DestroyNetworkObjects()
	{
		//var rpcBehaviours = GameObject.FindObjectsOfType<RPCPackagerBehavior>();
		//foreach (var rpcBehaviour in rpcBehaviours)
		//{
		//	rpcBehaviour.networkObject.Destroy();
		//}

		//foreach (var networkObject in NetworkObjects)
		//{
		//	networkObject.Destroy();
		//}
	}
}
