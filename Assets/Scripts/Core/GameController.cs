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
    using DG.Tweening;
    using Game.Camera;
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
        [SerializeField] private CameraFollower _camera;
        [SerializeField] private UIGameplay _uiGameplay;
        [SerializeField] private Button _startFindingBtn;
        [SerializeField] private List<GameObject> _spawnerPoint = new();

        //connection
        [SerializeField] private List<ulong> _connectionList = new();
        public Dictionary<ulong, PlayerInRoom> PlayerList = new();
        private bool _startGame;
        private float _time;
        private float _gameplayTime = 30f;
        private Tweener _gameTimeTweener;

        private void Start()
        {
            if (Instance == null)
                Instance = this;

            _camera = CameraFollower.Instance;
        }

        private void Update()
        {
            _uiGameplay.UpdateTime(_time.ToString("#"));

            if (_startGame)
            {
                _time -= Time.deltaTime;
                if (_time <= 0)
                {
                    if (IsHost)
                    {
                        StartHideAndSeek();
                        foreach (var player in PlayerList.Values)
                        {
                            if (!player.Player.IsHider.Value)
                            {
                                player.Player.StartMoveClientRpc();
                            }
                        }
                        StartGamePlayClientRpc();
                    }
                    _startGame = false;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;


            }

            if (IsServer && _startFindingBtn != null)
                _startFindingBtn.onClick.AddListener(() =>
                {
                    RandomChoosePlayer();
                });

        }
        public void StartGame()
        {
            if (IsServer)
                RandomChoosePlayer();
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            }

        }

        public Transform GetPointToSpawn()
        {
            return _spawnerPoint[_connectionList.Count].transform;
        }


        private void OnClientConnectedCallback(ulong clientId)
        {
            if (IsServer)
                _connectionList.Add(clientId);
        }

        private void OnClientDisconnectCallback(ulong clientId)
        {
            UpdatePlayerOutClientRpc(clientId);
        }
        [ClientRpc]
        private void UpdatePlayerOutClientRpc(ulong clientId)
        {
            PlayerList.Remove(clientId);
            _connectionList.Remove(clientId);
            _uiGameplay.UpdateTargetList(PlayerList);
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
            PlayerList.Clear();
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
                        }
                        else
                        {
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
            });

            StartupGame();

        }

        public async void StartupGame()
        {
            await Task.Delay(1000);
            _camera.SetTarget(Player.LocalPlayer.gameObject);
            _camera.ZoomTo(Player.LocalPlayer.IsHider.Value ? 35 : 15, 1);
            _uiGameplay.DisplayGameStartUp(Player.LocalPlayer.IsHider.Value);
            _time = 7f;
            _startGame = true;
        }

        [ClientRpc]
        private void StartGamePlayClientRpc()
        {
            _uiGameplay.DisplayInPlayGame(PlayerList);
            _uiGameplay.TimeReminingIcon.fillAmount = 0;
            _gameTimeTweener = _uiGameplay.TimeReminingIcon.DOFillAmount(1, _gameplayTime)
                 .OnComplete(() =>
                 {
                     if (IsHost)
                     {
                         if (!CheckCatchedAllPlayer())
                             ResetGame(isHiderWin: true);
                     }
                 });
            _camera.ZoomTo(35, 1);
        }

        [ServerRpc(RequireOwnership = false)]
        public void KillPlayerServerRpc(ulong clientId)
        {
            if (PlayerList[clientId].WasCatching)
                return;

            PlayerList[clientId].Player.SetIsKill();
            PlayerList[clientId].WasCatching = true;
            UpdateCatchedClientRpc(clientId);

            if (CheckCatchedAllPlayer())
                ResetGame(isHiderWin: false);

        }

        [ClientRpc]
        public void UpdateCatchedClientRpc(ulong clientId)
        {
            if (!IsHost)
                PlayerList[clientId].WasCatching = true;

            _uiGameplay.UpdateTargetList(PlayerList);
        }

        private bool CheckCatchedAllPlayer()
        {

            bool catchedAll = true;
            foreach (var player in PlayerList.Values)
            {
                if (player.IsHider && player.WasCatching == false)
                    catchedAll = false;
            }
            return catchedAll;
        }

        //0 is seeker win
        private async void ResetGame(bool isHiderWin)
        {
            //reset position

            ResetUIAndSendResultClientRpc(isHiderWin);
            await Task.Delay(300);
            foreach (var player in PlayerList.Values)
            {
                player.Player.Reset();
            }
            
        }
        //UI
        [ClientRpc]
        public void ResetUIAndSendResultClientRpc(bool isHiderWin)
        {
            _uiGameplay.HideInPlayGameUI();
            _gameTimeTweener.Kill();
            bool wincheck = isHiderWin == Player.LocalPlayer.IsHider.Value;
            if (UIManager.Instance != null)
                DisplayResultUI(wincheck);
            else
                Debug.Log($"You {wincheck}");
        }

        private async void DisplayResultUI(bool winCheck)
        {
            await Task.Delay(2000);
            UIManager.Instance.ToggleView(ViewName.Lobby);
            UIManager.Instance.ShowPopup(PopupName.GameResult, new Dictionary<string, object>(){
                    {"result", winCheck}
                });
        }




    }

}
