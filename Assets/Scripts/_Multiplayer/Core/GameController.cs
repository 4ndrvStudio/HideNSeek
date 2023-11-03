using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace HS4
{
    public class GameController : NetworkBehaviour
    {
        public static GameController Instance;
        private int Count = 0;

        private void Start() {
            if(Instance==null)
                Instance = this;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            }
        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if (IsServer)
            {
                Count++;
                if (Count > 1)
                {
                    if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.TryGetComponent(out Player player))
                    {
                        player.SetIsHider();
                    }
                }
            }
        }

        [ServerRpc]
        public void KillPlayerServerRpc(ulong clientId) {

            if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.TryGetComponent(out Player player))
            {
                    player.SetIsKill();
            }
        }
    }

}
