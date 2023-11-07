using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public enum LobbyContent
    {
        Join,
        Create
    }
    public class View_Lobbies : UIView
    {
        [Header("Buttons")]
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _deactiveColor;
        [SerializeField] private Button _createLobbyBtn;
        [SerializeField] private Button _joinLobbyBtn;

        [Header("Contents")]
        [SerializeField] private GameObject _createContentOb;
        [SerializeField] private GameObject _joinContentOb;

        [Header("Create Contents")]
        [SerializeField] private TMP_InputField _lobbyNameInput;
        [SerializeField] private Button _createBtn;
        [SerializeField] private GameObject _createBtnText;
        [SerializeField] private GameObject _createLoadingOb;

        [Header("Join Contents")]
        [SerializeField] private GameObject _loadingIconOb;
        [SerializeField] private GameObject _lobbyPanelOb;
        [SerializeField] private GameObject _lobbyPanelHolder;
        [SerializeField] private List<GameObject> _lobbyList = new();


        private void Start()
        {
            _joinLobbyBtn.onClick.AddListener(() =>
            {
                ToggleContent(LobbyContent.Join);
            });
            _createLobbyBtn.onClick.AddListener(() =>
            {
                ToggleContent(LobbyContent.Create);
            });

            _createBtn.onClick.AddListener(async () =>
            {
                ToggleLoading(true);
                LobbyPlayerData playerData = new LobbyPlayerData()
                {
                    IsReady = false,
                    DisplayName = "custom",
                    CharacterType = 1
                };
                var createLobbyResult = await LobbyManager.Instance.CreateLobby(_lobbyNameInput.text, 6, playerData, false);
                if (createLobbyResult.IsSuccess)
                {
                    UIManager.Instance.ToggleView(ViewName.Lobby, new Dictionary<string, object>() { { "lobby", createLobbyResult.Data } });
                    _createLoadingOb.gameObject.SetActive(false);
                    Hide();
                }
                ToggleLoading(false);

            });
        }

        public override void Show(Dictionary<string, object> customProperties)
        {
            base.Show(customProperties);
            ToggleContent(LobbyContent.Join);

        }

        private void ToggleContent(LobbyContent lobbyContent)
        {
            _createContentOb.SetActive(false);
            _joinContentOb.SetActive(false);
            _createLobbyBtn.GetComponent<Image>().color = _deactiveColor;
            _joinLobbyBtn.GetComponent<Image>().color = _deactiveColor;

            switch (lobbyContent)
            {
                case LobbyContent.Join:
                    _joinContentOb.SetActive(true);
                    _joinLobbyBtn.GetComponent<Image>().color = _activeColor;
                    FecthLobbiesData();
                    break;
                case LobbyContent.Create:
                    _createContentOb.SetActive(true);
                    _createLobbyBtn.GetComponent<Image>().color = _activeColor;
                    ToggleLoading(false);
                    break;

            }
        }
        private void ToggleLoading(bool isLoad)
        {
            _createLoadingOb.SetActive(isLoad);
            _createBtnText.SetActive(!isLoad);
        }

        private async void FecthLobbiesData()
        {
            _loadingIconOb.SetActive(true);
            _lobbyList.ForEach(item => Destroy(item));
            _lobbyList.Clear();

            var lobbiesResult = await LobbyManager.Instance.RetrieveLobbyList();
            if (lobbiesResult.IsSuccess)
            {
                QueryResponse lobbiesRespone = lobbiesResult.Data as QueryResponse;
                lobbiesRespone.Results.ForEach(lobby =>
                {
                    GameObject lobbyPanel = Instantiate(_lobbyPanelOb, _lobbyPanelHolder.transform);
                    UILobbyItem lobbyItemScript = lobbyPanel.GetComponent<UILobbyItem>();
                    lobbyItemScript.Setup(lobby);
                    _lobbyList.Add(lobbyPanel);
                });
            }
            _loadingIconOb.SetActive(false);

        }






    }

}
