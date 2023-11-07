using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace HS4
{
    using PlayerCore;
    public class PlayerInRoom
    {
        public Player Player;
        public bool IsHider;
        public bool WasCatching;

        public PlayerInRoom(Player player, bool isHider)
        {
            Player = player;
            IsHider = isHider;
            WasCatching = false;
        }
    }


    public class GameController : NetworkBehaviour
    {
        public static GameController Instance;

        private Dictionary<ulong, PlayerInRoom> _playerList = new();

        private int Count = 0;

        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private Button _startFindingBtn;
        
        public NetworkVariable<float> Time = new NetworkVariable<float>();

        private NetworkVariable<bool> m_CountdownStarted = new NetworkVariable<bool>(false);

        private float time;

        private void Start()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Update() {
            _timeText.text = time.ToString();
        }

    

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
       
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            
            if(IsServer)
            _startFindingBtn.onClick.AddListener(() => {
                StartHideAndSeek();
            });
        }

        public  override void OnNetworkDespawn() {
            base.OnNetworkDespawn();
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;

        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.TryGetComponent(out Player player))
            {
                Count++;

                if (Count > 1)
                {
                    if (IsServer)
                        player.SetIsHider();

                    _playerList.Add(clientId, new PlayerInRoom(player, true));

                }
                else
                    _playerList.Add(clientId, new PlayerInRoom(player, false));
            }
        }

        public void StartHideAndSeek()
        {
                foreach (var player in _playerList.Values)
                {
                    player.Player.SetStartHideAndSeekClientRpc();
                } 
            
        }

        public void HideEnemy(bool _isHider)
        {
            Debug.Log(_isHider);
            if (!_isHider)
            {
                Debug.Log(_playerList.Count);

                foreach (var player in _playerList.Values)
                {
                    if(player.Player.IsHider.Value)
                         player.Player.PlayerView.Hide();
                    
                }
            }
        }

        [ServerRpc]
        public void KillPlayerServerRpc(ulong clientId)
        {
            _playerList[clientId].Player.SetIsKill();
        }


    }

}
