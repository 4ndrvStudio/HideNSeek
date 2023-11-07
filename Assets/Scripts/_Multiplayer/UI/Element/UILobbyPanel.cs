using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Services.Lobbies.Models;

namespace HS4.UI
{
    public class UILobbyPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _readyText;
        
        
        public void Setup(Dictionary<string, PlayerDataObject> playerData, bool hasPlayer ) {
            if(!hasPlayer) {
                _canvasGroup.alpha =0;
                return;
            } 
            _canvasGroup.alpha =1;
            _nameText.text = playerData["DisplayName"].Value;
            _readyText.text = playerData["IsReady"].Value;
        }
    }

}
