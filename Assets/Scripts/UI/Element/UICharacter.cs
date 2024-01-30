using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HS4.UI
{
    public class UICharacter : MonoBehaviour
    {
        public CharacterData CharacterData;
        [SerializeField] private GameObject _selectedOb;
        [SerializeField] private Button _btnSelect;
        [SerializeField] private Button _btnBuy;
        [SerializeField] private TextMeshProUGUI _selectText;
        [SerializeField] private GameObject _selectLoadingIcon;

        [SerializeField] private GameObject _buyLoadingIcon;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private GameObject _priceIcon;

        public bool IsTargetVew
        {
            set
            {
                if(CharacterData.IsInUse) {
                    _selectedOb.SetActive(true);
                    _btnSelect.gameObject.SetActive(false);
                    _btnBuy.gameObject.SetActive(false);
                } else {
                    _btnSelect.gameObject.SetActive(value && CharacterData.IsUnlocked);
                    _btnBuy.gameObject.SetActive(value && !CharacterData.IsUnlocked);
                    _priceText.text = value ? CharacterData.Price.ToString() : "";
                     _selectedOb.SetActive(false);
                }
                
            }
        }

        private bool _isExcuting
        {
            set 
            {   
                _selectLoadingIcon.SetActive(value);
                _buyLoadingIcon.SetActive(value);
                _selectText.gameObject.SetActive(!value);
                _priceText.gameObject.SetActive(!value);
                _priceIcon.gameObject.SetActive(!value);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            _btnBuy.onClick.AddListener(Buy);
            _btnSelect.onClick.AddListener(Select);
        }

        public void Setup(CharacterData characterData)
        {
            gameObject.SetActive(true);
            CharacterData = characterData;
            if(CharacterData.IsInUse) {
                CharacterData.Animator.Play("Dance");
            } else {
                 CharacterData.Animator.Play("Idle");
            }
        }

        private async void Select() {
            _isExcuting =true;
            var selectResult =  await User.SelectCharacter(CharacterData.Id);
            _isExcuting = false;
            
            if(selectResult.IsSuccess) {
                UISelectCharacter.Instance.RefreshSelect();
                CharacterData.IsInUse = true;
                CharacterData.Animator.Play("Dance");
            } else {
                var notifyData = new NotifyData {
                      IsSuccess =false,
                      Title = "Select Character Fail",
                      Detail = selectResult.Message
                };
                UIManager.Instance.ShowPopup(PopupName.Notify,new Dictionary<string, object>{{"notifyData", notifyData}});
            }
        }

        private async void Buy() {
           
            _isExcuting =true;
            var buyResult = await User.BuyCharacter(CharacterData.Id);
            _isExcuting = false;
            
            if(buyResult.IsSuccess) {
                CharacterData.IsUnlocked = true;
                 UIManager.Instance.CurrentView.RefreshBalance();
            } else {
                var notifyData = new NotifyData {
                      IsSuccess =false,
                      Title = "Buy Character Fail",
                      Detail = buyResult.Message
                };
                UIManager.Instance.ShowPopup(PopupName.Notify,new Dictionary<string, object>{{"notifyData", notifyData}});
            }
        }
    }

}

