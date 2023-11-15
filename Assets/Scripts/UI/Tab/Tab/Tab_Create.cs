using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
namespace HS4.UI
{
    public class Tab_Create : UITab
    {

        [SerializeField] private TMP_InputField _lobbyNameInput;
        [SerializeField] private Button _createBtn;
        [SerializeField] private GameObject _createBtnText;
        [SerializeField] private GameObject _createLoadingOb;
        [SerializeField] private Toggle _privateToggle;


        private void Start() {
              _createBtn.onClick.AddListener(async () =>
            {
                ToggleLoading(true);
                LobbyPlayerData playerData = new LobbyPlayerData()
                {
                    IsHost = true,
                    DisplayName = User.Info.UserName,
                    CharacterType = 1
                };
                var createLobbyResult = await LobbyManager.Instance.CreateLobby(_lobbyNameInput.text, 6, playerData, _privateToggle.isOn);
                if (createLobbyResult.IsSuccess)
                {
                    UIManager.Instance.ToggleView(ViewName.Lobby, new Dictionary<string, object>() { { "lobby", createLobbyResult.Data } });
                    _createLoadingOb.gameObject.SetActive(false);
                }
                ToggleLoading(false);
            });

        }

         private void ToggleLoading(bool isLoad)
        {
            _createLoadingOb.SetActive(isLoad);
            _createBtnText.SetActive(!isLoad);
        }

   
    }

}
