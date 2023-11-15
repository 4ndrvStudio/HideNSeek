using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
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
        [SerializeField] private Button _homeBtn;

        [SerializeField] private List<UITabButton> _lobbiesTabButton = new();
        [SerializeField] private List<UITab> _lobliesTabList = new();

        private void Awake()
        {
            _homeBtn.onClick.AddListener(() => UIManager.Instance.ToggleView(ViewName.Home));
            
            _lobbiesTabButton.ForEach(tabButton => {
                tabButton.Button.onClick.AddListener(() => {
                    _lobbiesTabButton.ForEach(tabButton => tabButton.Deactive());
                    _lobliesTabList.ForEach(tab => tab.Deactive());
                    var targetTab = _lobliesTabList.Find(tab => tab.TabName == tabButton.TabName);
                    targetTab.Active();
                    tabButton.Active();
                });
            });
        }

        public override void Show(Dictionary<string, object> customProperties)
        {
            base.Show(customProperties);
            _lobbiesTabButton[0].Button.onClick.Invoke();
        }

    






    }

}
