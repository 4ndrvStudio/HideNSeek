using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

namespace HS4.UI
{
    public class View_Lobby : UIView
    {
        [Header("Players")]
        [SerializeField] private Button _leaveBtn;
        [SerializeField] private Button _readyBtn;

        [Header("Player Panel")]
        [SerializeField] private GameObject _lobbyPanelItem;
        [SerializeField] private GameObject _lobbyPanelHolder;
        [SerializeField] private List<GameObject> _lobbyUserList  = new(6);

        private Lobby _lobby;
        private ILobbyEvents _lobbyEvents;

        public override async void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            Lobby lobby = customProperties["lobby"] as Lobby;
            _lobby = lobby;

            FetchData();

            //Subscribe Events
            var eventCallbacks = new LobbyEventCallbacks();
            eventCallbacks.LobbyChanged += OnLobbyChanged;

            _lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(lobby.Id, eventCallbacks);
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
                // Handle lobby being deleted
                // Calling changes.ApplyToLobby will log a warning and do nothing
            }
            else
            {
                changes.ApplyToLobby(_lobby);
            }

            FetchData();

        }

        private void FetchData() 
        {
            for(int i = 0; i<= _lobbyUserList.Count; i++) {
                
            }
        }


    }

}
