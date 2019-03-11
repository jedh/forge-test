using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.Generated
{
	[GeneratedRPC("{\"types\":[[\"byte[]\"][\"ushort\", \"ushort\", \"string\", \"uint\"][\"ushort\", \"ushort\", \"string\"][\"ushort\", \"ushort\", \"uint\"][\"ushort\", \"ushort\"]]")]
	[GeneratedRPCVariableNames("{\"types\":[[\"ByteData\"][\"PlayerID\", \"TeamID\", \"Name\", \"NetworkID\"][\"PlayerID\", \"TeamID\", \"Name\"][\"PlayerID\", \"TeamID\", \"NetworkID\"][\"PlayerID\", \"TeamID\"]]")]
	public abstract partial class RPCPackagerBehavior : NetworkBehavior
	{
		public const byte RPC_SEND_PACKAGE = 0 + 5;
		public const byte RPC_PLAYER_JOINED_GAME = 1 + 5;
		public const byte RPC_UPDATE_PLAYER_NAME = 2 + 5;
		public const byte RPC_PLAYER_LEFT_GAME = 3 + 5;
		public const byte RPC_PLAYER_CHANGE_TEAMS = 4 + 5;
		
		public RPCPackagerNetworkObject networkObject = null;

		public override void Initialize(NetworkObject obj)
		{
			// We have already initialized this object
			if (networkObject != null && networkObject.AttachedBehavior != null)
				return;
			
			networkObject = (RPCPackagerNetworkObject)obj;
			networkObject.AttachedBehavior = this;

			base.SetupHelperRpcs(networkObject);
			networkObject.RegisterRpc("SendPackage", SendPackage, typeof(byte[]));
			networkObject.RegisterRpc("PlayerJoinedGame", PlayerJoinedGame, typeof(ushort), typeof(ushort), typeof(string), typeof(uint));
			networkObject.RegisterRpc("UpdatePlayerName", UpdatePlayerName, typeof(ushort), typeof(ushort), typeof(string));
			networkObject.RegisterRpc("PlayerLeftGame", PlayerLeftGame, typeof(ushort), typeof(ushort), typeof(uint));
			networkObject.RegisterRpc("PlayerChangeTeams", PlayerChangeTeams, typeof(ushort), typeof(ushort));

			networkObject.onDestroy += DestroyGameObject;

			if (!obj.IsOwner)
			{
				if (!skipAttachIds.ContainsKey(obj.NetworkId)){
					uint newId = obj.NetworkId + 1;
					ProcessOthers(gameObject.transform, ref newId);
				}
				else
					skipAttachIds.Remove(obj.NetworkId);
			}

			if (obj.Metadata != null)
			{
				byte transformFlags = obj.Metadata[0];

				if (transformFlags != 0)
				{
					BMSByte metadataTransform = new BMSByte();
					metadataTransform.Clone(obj.Metadata);
					metadataTransform.MoveStartIndex(1);

					if ((transformFlags & 0x01) != 0 && (transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() =>
						{
							transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform);
							transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform);
						});
					}
					else if ((transformFlags & 0x01) != 0)
					{
						MainThreadManager.Run(() => { transform.position = ObjectMapper.Instance.Map<Vector3>(metadataTransform); });
					}
					else if ((transformFlags & 0x02) != 0)
					{
						MainThreadManager.Run(() => { transform.rotation = ObjectMapper.Instance.Map<Quaternion>(metadataTransform); });
					}
				}
			}

			MainThreadManager.Run(() =>
			{
				NetworkStart();
				networkObject.Networker.FlushCreateActions(networkObject);
			});
		}

		protected override void CompleteRegistration()
		{
			base.CompleteRegistration();
			networkObject.ReleaseCreateBuffer();
		}

		public override void Initialize(NetWorker networker, byte[] metadata = null)
		{
			Initialize(new RPCPackagerNetworkObject(networker, createCode: TempAttachCode, metadata: metadata));
		}

		private void DestroyGameObject(NetWorker sender)
		{
			MainThreadManager.Run(() => { try { Destroy(gameObject); } catch { } });
			networkObject.onDestroy -= DestroyGameObject;
		}

		public override NetworkObject CreateNetworkObject(NetWorker networker, int createCode, byte[] metadata = null)
		{
			return new RPCPackagerNetworkObject(networker, this, createCode, metadata);
		}

		protected override void InitializedTransform()
		{
			networkObject.SnapInterpolations();
		}

		/// <summary>
		/// Arguments:
		/// byte[] ByteData
		/// </summary>
		public abstract void SendPackage(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// ushort PlayerID
		/// ushort TeamID
		/// string Name
		/// uint NetworkID
		/// </summary>
		public abstract void PlayerJoinedGame(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// ushort PlayerID
		/// ushort TeamID
		/// string Name
		/// </summary>
		public abstract void UpdatePlayerName(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// ushort PlayerID
		/// ushort TeamID
		/// uint NetworkID
		/// </summary>
		public abstract void PlayerLeftGame(RpcArgs args);
		/// <summary>
		/// Arguments:
		/// ushort PlayerID
		/// ushort TeamID
		/// </summary>
		public abstract void PlayerChangeTeams(RpcArgs args);

		// DO NOT TOUCH, THIS GETS GENERATED PLEASE EXTEND THIS CLASS IF YOU WISH TO HAVE CUSTOM CODE ADDITIONS
	}
}