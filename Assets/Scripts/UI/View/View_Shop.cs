using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HS4.Config;
namespace HS4.UI
{
    public class View_Shop : UIView
    {
        [SerializeField] private GameObject _shopContent;
        [SerializeField] private GameObject _loadingCircle;
        [SerializeField] private List<UIGoldPack> _goldPackList = new();
        [SerializeField] private List<UIGemPack> _gemPackList = new();

        [SerializeField] private Button _homeBtn;

        private bool _isloading
        {
            set
            {
                _loadingCircle.SetActive(value);
                _shopContent.SetActive(!value);
            }
        }

        private void Start()
        {
            _homeBtn.onClick.AddListener(() => UIManager.Instance.ToggleView(ViewName.Home));
        }
        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            FetchData();
        }

        public void FetchData()
        {
            _isloading = true;

            //gold pack
            var goldConfig = ConfigManager.Instance.BundlePackConfig.GoldPacks;

            for (int i = 0; i< goldConfig.Count; i++)
            {
                _goldPackList[i].Setup(goldConfig[i]);
            }

            //gem pack
            var gemConfig = ConfigManager.Instance.BundlePackConfig.GemPacks;

            for(int i = 0; i< gemConfig.Count; i++)
            {
                _gemPackList[i].Setup(gemConfig[i]);
            }

            _isloading = false;
        }
        
    }

}
