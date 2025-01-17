using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public enum PopupName
    {
        None,
        GameResult,
        UserInfo,
        SetName,
        Settings,
        EnterLobby,
        Reward,
        Notify
    }

    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private PopupName _popupName = PopupName.None;
        public PopupName PopupName => _popupName;

        protected Dictionary<string, object> _customProperties;


        public virtual void Show(Dictionary<string, object> customProperties = null)
        {
            this._customProperties = customProperties;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            _customProperties = null;
            gameObject.SetActive(false);
        }
    }

}

