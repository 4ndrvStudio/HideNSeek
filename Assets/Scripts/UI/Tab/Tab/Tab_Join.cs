using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.CloudSave.Models;


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

        private void Start()
        {
            _refreshBtn.onClick.AddListener(FecthLobbiesDataWithLoading);
            _enterBtn.onClick.AddListener(()=> UIManager.Instance.ShowPopup(PopupName.EnterLobby));
        }

        public override void Active()
        {
            base.Active();
            FecthLobbiesDataWithLoading();
        }

        public override void Deactive()
        {
            base.Deactive();
            _lobbyList.ForEach(item => Destroy(item));
            _lobbyList.Clear();
            StopAllCoroutines();
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private async void FecthLobbiesDataWithLoading()
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
            StopAllCoroutines();
            StartCoroutine(AutoFecthLobbiesData(3));
        }


        private IEnumerator AutoFecthLobbiesData(float waitTimeSeconds)
        {
            var delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true)
            {
                yield return delay;
                FecthLobbiesData();
            }
        }

        private async void FecthLobbiesData()
        {
            var lobbiesResult = await LobbyManager.Instance.RetrieveLobbyList();
            if (lobbiesResult.IsSuccess)
            {
                QueryResponse lobbiesRespone = lobbiesResult.Data as QueryResponse;

                //Remove item 
                List<GameObject> itemsToRemove = new List<GameObject>();

                foreach (var item in _lobbyList)
                {
                    int index = lobbiesRespone.Results.FindIndex(lobbyResult => lobbyResult.Id == item.GetComponent<UILobbiesPanel>().Lobby.Id);
                    if (index == -1 && item != null)
                    {
                        itemsToRemove.Add(item);
                    }
                }

                foreach (var itemToRemove in itemsToRemove)
                {
                    _lobbyList.Remove(itemToRemove);
                    Destroy(itemToRemove);
                }

                //add or modify item
                lobbiesRespone.Results.ForEach(lobby =>
                {
                    int index = _lobbyList.FindIndex(item => item.GetComponent<UILobbiesPanel>().Lobby.Id == lobby.Id);
                    if (index == -1)
                    {
                        GameObject lobbyPanel = Instantiate(_lobbyPanelOb, _lobbyPanelHolder.transform);
                        UILobbiesPanel lobbyItemScript = lobbyPanel.GetComponent<UILobbiesPanel>();
                        lobbyItemScript.Setup(lobby);
                        _lobbyList.Add(lobbyPanel);
                    }
                    else
                    {
                        _lobbyList[index].GetComponent<UILobbiesPanel>().Setup(lobby);
                    }
                });




            }
        }

    }

}
