using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class UILobbiesPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _lobbyNameText;
        [SerializeField] private TextMeshProUGUI _playerCountText;
        [SerializeField] private Slider _slider;
        [SerializeField] private Button _joinBtn;
        [SerializeField] private Image _lockIcon;
        [SerializeField] private Color _lockNormalColor;


        public Lobby Lobby;

        // Start is called before the first frame update
        void Start()
        {
            
            _joinBtn.onClick.AddListener( async () => {
                LobbyPlayerData lobbyPlayerData = new LobbyPlayerData() 
                {
                    DisplayName = User.Info.UserName,
                    CharacterType = 1
                };
           
                var joinResult = await LobbyManager.Instance.JoinLobby(Lobby.Id,lobbyPlayerData);
              
                if(joinResult.IsSuccess) {
                    UIManager.Instance.ToggleView(ViewName.Lobby, 
                    new Dictionary<string, object>() {{"lobby",joinResult.Data}});
                }
            });
        }

        public void Setup(Lobby lobby) {
            this.gameObject.SetActive(true);
            Lobby = lobby;
            _lockIcon.color = Lobby.IsLocked ? Color.white: _lockNormalColor;
            _lobbyNameText.text = lobby.Name + "'s Lobby";
            _playerCountText.text = $"{lobby.Players.Count.ToString()}<#557190>/{lobby.MaxPlayers.ToString()}";
            _slider.value = (float)lobby.Players.Count/(float)lobby.MaxPlayers;
        }

        
    }

}
