using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace HS4.UI
{
    public class Popup_EnterLobby : UIPopup
    {
        [SerializeField] private Button _closeBtn;
        [SerializeField] private Button _okBtn;
        [SerializeField] private TMP_InputField _lobbyInput;
        [SerializeField] private TextMeshProUGUI _notifyText;
        [SerializeField] private GameObject _spinnerIcon;
        [SerializeField] private GameObject _okBtnText;

        // Start is called before the first frame update
        void Start()
        {
            _closeBtn.onClick.AddListener(Hide);
            _okBtn.onClick.AddListener(EnterLobby);
        }

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
             _notifyText.gameObject.SetActive(false);
        }

        private async void EnterLobby() 
        {
            SetLoading(true);
            _notifyText.gameObject.SetActive(false);
            LobbyPlayerData lobbyPlayerData = new LobbyPlayerData() 
                {
                    DisplayName = User.Info.UserName,
                    CharacterType = 1
            };

            var result = await LobbyManager.Instance.QuickJoinLobby(_lobbyInput.text.Trim(),lobbyPlayerData);
            SetLoading(false);
            if(result.IsSuccess) {
                UIManager.Instance.ToggleView(ViewName.Lobby, 
                    new Dictionary<string, object>() {{"lobby",result.Data}});
                Hide();
            } else {
                string originalString = result.Message;
                string stringWithUpperCaseFirstLetter = char.ToUpper(originalString[0]) + originalString.Substring(1);
                _notifyText.text = stringWithUpperCaseFirstLetter;
                _notifyText.gameObject.SetActive(true);
            }
       
        }

        public void SetLoading(bool isLoading) {
            _spinnerIcon.SetActive(isLoading);
            _okBtnText.SetActive(!isLoading);
        }

        
    }

}
