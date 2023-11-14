using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;


namespace HS4.UI
{
    public class Tab_Join : UITab
    {
        [SerializeField] private GameObject _loadingIconOb;
        [SerializeField] private GameObject _lobbyPanelOb;
        [SerializeField] private GameObject _lobbyPanelHolder;
        [SerializeField] private List<GameObject> _lobbyList = new();

        [Header("Buttons")]
        [SerializeField] private Button _refreshBtn;
        [SerializeField] private Button _enterBtn;
        [SerializeField] private Button _filterBtn;

        private void Start() {
            _refreshBtn.onClick.AddListener(FecthLobbiesData);
        }

        public override void Active()
        {
            base.Active();
            FecthLobbiesData();
        }

        public override void Deactive()
        {
            base.Deactive();
            _lobbyList.ForEach(item => Destroy(item));
            _lobbyList.Clear();
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
                    UILobbiesPanel lobbyItemScript = lobbyPanel.GetComponent<UILobbiesPanel>();
                    lobbyItemScript.Setup(lobby);
                    _lobbyList.Add(lobbyPanel);
                });
            }
            _loadingIconOb.SetActive(false);
        }

    }

}
