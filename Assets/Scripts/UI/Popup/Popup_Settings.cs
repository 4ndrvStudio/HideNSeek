using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HS4.UI
{
    public class Popup_Settings : UIPopup
    {
        [SerializeField] private Button _closeBtn;

        private void Start() {
            _closeBtn.onClick.AddListener(Hide);
        }

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            

        }

    }

}
