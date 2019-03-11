using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LocalServer : MonoBehaviour
{
	public string IPAddress = "127.0.0.1";
	public ushort Port = 15937;
	public GameObject NetworkManagerObj;
	public string NatServerHost = string.Empty;
	public ushort NatServerPort = 15941;

	private NetworkManager m_Manager;
	private NetWorker m_Server;

	private void Awake()
	{
		//GameObject.DontDestroyOnLoad(this.gameObject);
	}

	private void Start()
	{
		Rpc.MainThreadRunner = MainThreadManager.Instance;
	}

	public void Host(string ip, ushort port, string matchName = "")
	{
		IPAddress = ip;
		Port = port;

		m_Server = new UDPServer(11);
		((UDPServer)m_Server).Connect(IPAddress, Port);

		Connected(m_Server);
	}

	public void Connect(string ipAddress, ushort port)
	{
		var client = new UDPClient();
		((UDPClient)client).Connect(ipAddress, port);

		Connected(client);
	}

	public void Connected(NetWorker networker)
	{
		if (!networker.IsBound)
		{
			Debug.LogError("NetWorker failed to bind.");
			return;
		}

		if (m_Manager == null && NetworkManagerObj == null)
		{
			Debug.LogWarning("A network manager was not provided, generating a new one instead");
			NetworkManagerObj = new GameObject("Network Manager");
			m_Manager = NetworkManagerObj.AddComponent<NetworkManager>();
		}
		else if (m_Manager == null)
		{
			m_Manager = Instantiate(NetworkManagerObj).GetComponent<NetworkManager>();
		}

		m_Manager.Initialize(networker);
		NetworkManager.Instance.automaticScenes = true;
		//NetworkObject.Flush(networker);

		if (networker.IsServer)
		{
			RPCPackagerBehavior rpcPackagerBehaviour = NetworkManager.Instance.InstantiateRPCPackager();
			GameController.Instance.AttachServer(networker, rpcPackagerBehaviour);
			SceneManager.LoadScene("LobbyScene");
		}
		else
		{
			GameController.Instance.AttachClient(networker);
			//networker.serverAccepted += (sender) =>
			//{
			//	MainThreadManager.Run(() =>
			//	{
			//		SceneManager.LoadScene("LobbyScene");
			//	});
			//};

			((UDPClient)networker).connectAttemptFailed += (sender) =>
			{
				MainThreadManager.Run(() =>
				{
					Debug.Log("Unable to connect to sever.");
					SceneManager.LoadScene(0);
				});
			};
		}
	}

	private void OnApplicationQuit()
	{
		NetWorker.EndSession();
		m_Server?.Disconnect(true);
	}
}
