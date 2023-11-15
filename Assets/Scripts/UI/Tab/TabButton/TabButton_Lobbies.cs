using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class TabButton_Lobbies : UITabButton
    {
        [SerializeField] private Image _tabIcon;
        [SerializeField] private GameObject _tabFocusIcon;
        [SerializeField] private TextMeshProUGUI _tabNameText;
        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _focusColor;

        public override void Active()
        {
          
            _tabFocusIcon.SetActive(true);
            _tabIcon.color = _focusColor;
            _tabNameText.color = _focusColor;
        }
        public override void Deactive()
        {
            _tabFocusIcon.SetActive(false);
            _tabIcon.color = _normalColor;
            _tabNameText.color = _normalColor;
        }
    }

}
