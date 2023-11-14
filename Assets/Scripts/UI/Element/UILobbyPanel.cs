using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Authentication;

namespace HS4.UI
{
    public class UILobbyPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _panelImg;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _readyText;
        [SerializeField] private GameObject _hostIcon;
        [SerializeField] private Button _kickPlayerBtn;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _notReadyColor;
        [SerializeField] private Color _notMineColor;
        private bool _canKick;
        private string _lobbyId;
        private string _playerId;

        private void Start() {
            _kickPlayerBtn.onClick.AddListener(async () => {
                if(!_canKick) return;
                await LobbyService.Instance.RemovePlayerAsync(_lobbyId,_playerId);
            });
        }

        public void Setup(Dictionary<string, PlayerDataObject> playerData,string lobbyId = null, string playerId = null,bool canKick = false) {
            _canvasGroup.alpha =1;
            _nameText.text = playerData["DisplayName"].Value;
            _readyText.text = playerData["IsReady"].Value;
            bool isHost = bool.Parse(playerData["IsHost"].Value);
            _hostIcon.SetActive(isHost);
            _kickPlayerBtn.gameObject.SetActive(!isHost && canKick);
            _readyText.color =  bool.Parse(playerData["IsReady"].Value) ? _readyColor : _notReadyColor;

            _canKick = canKick;
            _lobbyId = lobbyId;
            _playerId = playerId;

            _panelImg.color = playerId == AuthenticationService.Instance.PlayerId ? Color.white : _notMineColor;

        }
        public void Lock() {
            _canvasGroup.alpha =0;
        }
    }

}
