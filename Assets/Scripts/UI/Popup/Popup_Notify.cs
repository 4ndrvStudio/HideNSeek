using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HS4.UI
{
    public class NotifyData
    {
        public bool IsSuccess;
        public string Title;
        public string Detail;
    }
    public class Popup_Notify : UIPopup
    {
        [SerializeField] private Image _notifyIcon;
        [SerializeField] private Image _notifyBackground;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _detailText;
        [SerializeField] private Button _okBtn;

        [SerializeField] private Color _successColor;
        [SerializeField] private Color _failColor;
        [SerializeField] private Sprite _successIcon;
        [SerializeField] private Sprite _failIcon;

        // Start is called before the first frame update
        void Start()
        {
            _okBtn.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);

            var notifyData = customProperties["notifyData"] as NotifyData;

            if(notifyData.IsSuccess)
            {
                _notifyIcon.sprite = _successIcon;
                _titleText.color = _successColor;
                _notifyBackground.color = _successColor;

                _titleText.text = notifyData.Title;
                _detailText.text = notifyData.Detail;
            }
            else
            {
                _notifyIcon.sprite = _failIcon;
                _titleText.color = _failColor;
                _notifyBackground.color = _failColor;

                _titleText.text = notifyData.Title;
                _detailText.text = notifyData.Detail;
            }
            
        }
    }

}
