using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class View_Home : UIView
    {
        [Header("Buttons")]
        [SerializeField] private Button _playBtn;
        [SerializeField] private Button _shopBtn;
        [SerializeField] private Button _userInfoBtn;
        [SerializeField] private Button _settingBtn;

        [Header("User Info")]
        [SerializeField] private TextMeshProUGUI _userNameText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private Slider _expSlider;

        // Start is called before the first frame update
        void Start()
        {

            _playBtn.onClick.AddListener(() => UIManager.Instance.ToggleView(ViewName.Lobbies));
            _userInfoBtn.onClick.AddListener(() => UIManager.Instance.ShowPopup(PopupName.UserInfo));
            _settingBtn.onClick.AddListener(() => UIManager.Instance.ShowPopup(PopupName.Settings));
           // _shopBtn.onClick.AddListener(() => UIManager.Instance.ToggleView(ViewName.Shop));
        }

        public override async void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);

            await User.GetUserInfo();

            if(User.Info.UserName == null) {
                var popupParams = new Dictionary<string, object>() {{"isRequire", true}};
                UIManager.Instance.ShowPopup(PopupName.SetName,popupParams);
            } else {
                _userNameText.text = User.Info.UserName;
                _levelText.text = User.Info.Level.ToString();
                _expText.text = User.Info.Exp.ToString() + "/1000";
                _expSlider.value = (float) User.Info.Exp / 1000f;
            }

            
        }


    }

}
