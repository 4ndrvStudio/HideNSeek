using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;


namespace HS4.UI
{
    public class Popup_SetName : UIPopup
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _okBtn;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TextMeshProUGUI _notifyText;
        [SerializeField] private GameObject _spinnerIcon;
        [SerializeField] private GameObject _okBtnText;

        private bool _isRequire;

        // Start is called before the first frame update
        void Start()
        {
            _closeBtn.onClick.AddListener(Hide);
            _okBtn.onClick.AddListener(SetName);
        }

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            _isRequire = bool.Parse(customProperties["isRequire"].ToString());
            _closeBtn.gameObject.SetActive(!_isRequire);
            _notifyText.gameObject.SetActive(false);
            _okBtnText.SetActive(true);
        }

        public async void SetName() 
        {
            string name = _nameInput.text.Trim();

            if(!VerifyName(name)) return;

            SetLoading(true);
            var result = await User.SetName(name);
            SetLoading(false);

            if(result.IsSuccess) 
            {
                UIManager.Instance.ToggleView(ViewName.Home);
                if(!_isRequire)
                    UIManager.Instance.ShowPopup(PopupName.UserInfo);
                Hide();
            }
            else {
                _notifyText.gameObject.SetActive(true);
                _notifyText.text = result.Message;
            }
        }   
        public void SetLoading(bool isLoading) {
            _spinnerIcon.SetActive(isLoading);
            _okBtnText.SetActive(!isLoading);
        }

        public bool VerifyName(string name) {
            bool isCanSet = true;

            if(name.Length>=9 || name.Length<3) {
                isCanSet = false;
                _notifyText.text = "Your name must be between 3 and 9 characters in length.";
            }

            if (!Regex.IsMatch(name, "^[a-zA-Z0-9]+$")) {
                isCanSet = false;
                _notifyText.text = "Your name must only contain letters and numbers.";
            }
            if(!isCanSet)
                _notifyText.gameObject.SetActive(true);
            else 
                _notifyText.gameObject.SetActive(false);

            return isCanSet;
        }
    }

}
