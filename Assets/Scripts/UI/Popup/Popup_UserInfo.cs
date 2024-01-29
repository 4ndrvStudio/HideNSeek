using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Authentication;

namespace HS4.UI
{
    public class Popup_UserInfo : UIPopup
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _editNameBtn;

        [SerializeField] private TextMeshProUGUI _userIdText;
        [SerializeField] private TMP_InputField _nameText;
        [SerializeField] private TextMeshProUGUI _levelAvatarText;
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _expText;
        [SerializeField] private Slider _expSlider;

        private void Start() {
            _closeBtn.onClick.AddListener(Hide);

            _editNameBtn.onClick.AddListener(() => UIManager.Instance.ShowPopup(PopupName.SetName,
                new Dictionary<string, object> { { "isRequire", false} }
                ));
        }

        public override async void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            
            Setup();
            
            await User.GetUserInfo();

            Setup();
            
            
        }

        private void Setup() {
            _userIdText.text  = "#"+AuthenticationService.Instance.PlayerId.Substring(0,7);
            _nameText.text =  User.Info.UserName;
            _levelAvatarText.text = User.Info.Level.ToString();
            _levelText.text = $"LV <#94aed0><size=170%>{User.Info.Level.ToString()}";
            _expText.text = User.Info.Exp.ToString() + "/1000";
            _expSlider.value = (float)User.Info.Exp / 1000f;
        }
    }

}
