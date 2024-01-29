using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4.UI
{
    public enum ViewName
    {
        None,
        Home,
        Lobbies,
        Lobby,
        Shop
    }
    public class UIView : MonoBehaviour
    {
        [SerializeField] private ViewName _viewName = ViewName.None;
        public ViewName ViewName => _viewName;

        [SerializeField] private UIBalance _balance;

        public virtual void Show(Dictionary<string, object> customProperties = null)
        {
            gameObject.SetActive(true);
           
            RefreshBalance();
        }

        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void RefreshBalance()
        {
            if (_balance == null)
                return;
            _balance.UpdateBalance();
        }
    }
}

