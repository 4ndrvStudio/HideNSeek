using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;
using TMPro;

namespace HS4.UI
{
    public class View_Lobby : UIView
    {

        [Header("Lobby")]
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _lobbyCodeText;

        [Header("Buttons")]
        [SerializeField] private Button _leaveBtn;
        [SerializeField] private Button _readyBtn;
        [SerializeField] private Color _isReadyColor;
        [SerializeField] private Color _notReadyColor;

        [Header("Player Panel")]
        [SerializeField] private List<UILobbyPanel> _lobbyUserList = new(6);
        [SerializeField] private GameObject _timePopup;
        [SerializeField] private TextMeshProUGUI _timeText;

        private Lobby _lobby;
        private ILobbyEvents _lobbyEvents;

        private float _time = 5f;
        private bool _startCalTime;

        private void Start()
        {
            _leaveBtn.onClick.AddListener(() =>
            {
                LeaveLobby();
            });
            _readyBtn.onClick.AddListener(() => ChangeStateReady());
        }

        private void Update()
        {
            if (_startCalTime)
            {
                _time -= Time.deltaTime;
                _timeText.text = $"The game will start in : {_time.ToString("#")}";
                if (_time <= 0)
                {
                    Hide();
                    if (GameController.Instance != null)
                        GameController.Instance.StartGame();
                    _time = 5f;
                    _startCalTime = false;
                }
            }
        }

        public override async void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            if (customProperties != null)
            {
                Lobby lobby = customProperties["lobby"] as Lobby;
                _lobby = lobby;
              
                _lobbyNameText.text = _lobby.Name + "'s Lobby";
                _lobbyCodeText.text = "Lobby Code: "+ _lobby.LobbyCode +"<#557190>";
                //Subscribe Events
                var eventCallbacks = new LobbyEventCallbacks();
                eventCallbacks.LobbyChanged += OnLobbyChanged;
                eventCallbacks.KickedFromLobby += OnKickedFromLobby;
                _lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(_lobby.Id, eventCallbacks);

            }
            else
                ChangeStateReady(forceNotReady: true);

            _time = 5f;
            _startCalTime = false;

            FetchData();


        }

        public override void Hide()
        {
            this._lobbyEvents = null;
            base.Hide();
        }

        private void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                Debug.Log("Room Has been Removed!");
                string playerId = AuthenticationService.Instance.PlayerId;

                if (_lobby != null)
                {
                    LobbyManager.Instance.LeaveLobby();
                    UIManager.Instance.ToggleView(ViewName.Lobbies);
                }

                return;


            }
            else
            {
                changes.ApplyToLobby(_lobby);
            }
            Debug.Log("Lobby change");
            FetchData();

        }

        private void OnKickedFromLobby()
        {
            this._lobbyEvents = null;
            LobbyManager.Instance.LeaveLobby();
            UIManager.Instance.ToggleView(ViewName.Lobbies);
        }


        private async void ChangeStateReady(bool forceNotReady = false)
        {
            try
            {
                string playerId = AuthenticationService.Instance.PlayerId;
                var player = _lobby.Players.Find(player => player.Id == playerId);

                UpdatePlayerOptions options = new UpdatePlayerOptions();
                bool isReady = forceNotReady ? false : !bool.Parse(player.Data["IsReady"].Value);
                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "IsReady", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: isReady.ToString())
                    }
                };
                _readyBtn.GetComponent<Image>().color = isReady ? _notReadyColor : _isReadyColor;
                _readyBtn.GetComponentInChildren<TextMeshProUGUI>().text = isReady ? "Not Ready" : "Ready";
                _lobby = await LobbyService.Instance.UpdatePlayerAsync(_lobby.Id, playerId, options);

                FetchData();

            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

        private void FetchData()
        {
            bool allReady = true;
            for (int i = 0; i < _lobbyUserList.Count; i++)
            {
                if (i < _lobby.Players.Count)
                {
                  
                    _lobbyUserList[i].Setup(_lobby.Players[i].Data, _lobby.Id, _lobby.Players[i].Id, _lobby.HostId == AuthenticationService.Instance.PlayerId);
                  
                    if (bool.Parse(_lobby.Players[i].Data["IsReady"].Value) != true)
                    {
                        allReady = false;
                    }
                }
                else
                    _lobbyUserList[i].Lock();
            }
            if (allReady && _lobby.Players.Count > 1)
            {
                _timePopup.SetActive(true);
                _startCalTime = true;
                _time = 5f;

            }
            else
            {
                _timePopup.SetActive(false);
                _startCalTime = false;
            }


        }

        private async void LeaveLobby()
        {
            try
            {
                if (_lobby != null)
                {
                    string playerId = AuthenticationService.Instance.PlayerId;
                    if (_lobby.HostId == playerId)
                    {
                        await LobbyService.Instance.DeleteLobbyAsync(_lobby.Id);
                    }
                    else
                    {
                        await LobbyService.Instance.RemovePlayerAsync(_lobby.Id, playerId);
                    }
                    UIManager.Instance.ToggleView(ViewName.Lobbies);
                    LobbyManager.Instance.LeaveLobby();
                    _lobby = null;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }


    }

}
