using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Views")]
        [SerializeField] private List<UIView> _listView = new List<UIView>();
        [SerializeField] private List<UIPopup> _listPopup = new List<UIPopup>();

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public void ShowPopup(PopupName popupName, Dictionary<string, object> customProperties = null)
        {
            UIPopup selectedPopup = _listPopup.Find(popup => popup.PopupName == popupName);
            if (selectedPopup != null) selectedPopup.Show(customProperties);

        }
        public void HidePopup(PopupName popupName)
        {
            UIPopup selectedPopup = _listPopup.Find(popup => popup.PopupName == popupName);
            if (selectedPopup != null) selectedPopup.Hide();
        }

        public void ToggleView(ViewName viewName, Dictionary<string, object> customProperties = null)
        {
            UIView selectedPanel = _listView.Find(tab => tab.ViewName == viewName);
            if (selectedPanel != null)
            {
                _listView.ForEach(view => view.Hide());
                selectedPanel.Show(customProperties);
            }
        }
    }
}
