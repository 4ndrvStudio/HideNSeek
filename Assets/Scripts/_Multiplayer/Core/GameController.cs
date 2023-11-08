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

        private void Update()
        {
            _timeText.text = time.ToString();
        }



        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;

            if (IsServer)
                _startFindingBtn.onClick.AddListener(() =>
                {
                    RandomChoosePlayer();
                });
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;

        }

        private void OnClientConnectedCallback(ulong clientId)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.TryGetComponent(out Player player))
            {
                // Count++;
                _playerList.Add(clientId, new PlayerInRoom(player, true));
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
            if (!_isHider)
            {
                Debug.Log(_playerList.Count);

                foreach (var player in _playerList.Values)
                {
                    if (player.Player.IsHider.Value)
                        player.Player.PlayerView.Hide();

                }
            }
        }

        private void RandomChoosePlayer()
        {
            List<KeyValuePair<ulong, PlayerInRoom>> playerList = new List<KeyValuePair<ulong, PlayerInRoom>>(_playerList);

            System.Random random = new System.Random();
            playerList.Sort((x, y) => random.Next(-1, 2));
            
            playerList[1].Value.IsHider = false;
            if(playerList.Count > 3)
                playerList[2].Value.IsHider = false;

            _playerList = new Dictionary<ulong, PlayerInRoom>();
          
            foreach (var kvp in playerList)
            {
                _playerList.Add(kvp.Key, kvp.Value);
                
                if(kvp.Value.IsHider) {
                    if(IsServer)
                    kvp.Value.Player.SetIsHider();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void KillPlayerServerRpc(ulong clientId)
        {
            _playerList[clientId].Player.SetIsKill();
        }


    }

}
