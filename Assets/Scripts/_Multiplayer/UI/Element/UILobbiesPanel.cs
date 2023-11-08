using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class UILobbiesPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _playerCountText;
        [SerializeField] private Button _joinBtn;

        private Lobby _lobby;

        // Start is called before the first frame update
        void Start()
        {
            _joinBtn.onClick.AddListener( async () => {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData() 
                {
                    DisplayName = "Join",
                    CharacterType = 1
                };
                var joinResult = await LobbyManager.Instance.JoinLobby(_lobby.Id,lobbyPlayerData);
                if(joinResult.IsSuccess) {
                    UIManager.Instance.ToggleView(ViewName.Lobby, 
                                new Dictionary<string, object>() {{"lobby",joinResult.Data}});
                }
            });
        }

        public void Setup(Lobby lobby) {
            this.gameObject.SetActive(true);
            _lobby = lobby;
            _lobbyNameText.text = lobby.Name + "'s Lobby";
            _playerCountText.text =  lobby.Players.Count.ToString()+"/" + lobby.MaxPlayers.ToString();
        }

        
    }

}
