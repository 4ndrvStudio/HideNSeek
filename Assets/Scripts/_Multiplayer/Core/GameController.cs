using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
namespace HS4
{
    using System;
    using System.Threading.Tasks;
    using HS4.UI;
    using PlayerCore;
    using Unity.Collections;

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
        [SerializeField] private UIGameplay _uiGameplay;
        [SerializeField] private GameObject _startgamePanel;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _characterTypeText;
        [SerializeField] private Button _startFindingBtn;

        //connection
        [SerializeField] private List<ulong> _connectionList = new();
        public Dictionary<ulong, PlayerInRoom> PlayerList = new();
        private bool _startGame;
        private float _time;


        private void Start()
        {
            if (Instance == null)
                Instance = this;
        }

        private void Update()
        {
            _timeText.text = _time.ToString("#");
            if (_startGame)
            {
                _time -= Time.deltaTime;
                if (_time <= 0)
                {
                    if (IsHost)
                    {
                        StartHideAndSeek();
                        foreach(var player in PlayerList.Values) {
                            if(!player.Player.IsHider.Value) {
                                player.Player.StartMoveClientRpc();
                            }
                        }
                    }

                    _startgamePanel.SetActive(false);
                    _startGame = false;
                }
            }
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
            if (IsServer)
                _connectionList.Add(clientId);
        }

        public void StartHideAndSeek()
        {
            foreach (var player in PlayerList.Values)
            {
                player.Player.SetStartHideAndSeekClientRpc();   
            }

        }


        private void RandomChoosePlayer()
        {
            if (IsServer == false) return;

            System.Random random = new System.Random();
            _connectionList.Sort((x, y) => random.Next(-1, 2));


            SendListClientRpc(_connectionList.ToArray());
        }


        [ClientRpc]
        public void SendListClientRpc(ulong[] connectionList)
        {
            int count = connectionList.Length > 3 ? 2 : 1;
            _connectionList = connectionList.ToList();
            connectionList.ToList<ulong>().ForEach(clientId =>
            {

                if (IsHost)
                {
                    if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.TryGetComponent(out Player player))
                    {
                        PlayerList.Add(clientId, new PlayerInRoom(player, true));
                        if (count > 0)
                        {
                            PlayerList[clientId].IsHider = false;
                            player.SetIsSeeker();
                            count--;
                        } else {
                            player.StartMoveClientRpc();
                        }
                    }
                }
                else
                {
                    PlayerList.Add(clientId, new PlayerInRoom(null, true));
                    if (count > 0)
                    {
                        PlayerList[clientId].IsHider = false;
                        count--;
                    }
                }

                count--;
            });

            StartGameSetup();
        
        }

        public async void StartGameSetup()
        {
            await Task.Delay(1000);
            if (!Player.LocalPlayer.IsHider.Value)
            {
                //Player focus camera
            }
           

            DisplayUIAndCalTime();
        }

        private void DisplayUIAndCalTime()
        {
            _startgamePanel.SetActive(true);
            _characterTypeText.text = Player.LocalPlayer.IsHider.Value ? "You are Hider!" : "You Are Seeker";
            _time = 7f;
            _startGame = true;
        }

        [ClientRpc]
        private void DisplayGamePlayUIClientRpc() 
        {

        }

        [ServerRpc(RequireOwnership =false)]
        public void KillPlayerServerRpc(ulong clientId)
        {
            PlayerList[clientId].Player.SetIsKill();
            PlayerList[clientId].WasCatching = true;
            CheckCatchedAllPlayer();
        }

        private void CheckCatchedAllPlayer() {

            bool catchedAll = true;
            foreach (var player in PlayerList.Values)
            {
                if(player.IsHider && player.WasCatching ==false)
                    catchedAll = false;
            }   

            if(catchedAll)
                Debug.Log("Engame");
            
        }


    }

}
