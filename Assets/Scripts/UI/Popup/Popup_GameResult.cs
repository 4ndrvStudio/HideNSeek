using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace HS4.UI
{
    public class Popup_GameResult : UIPopup
    {
        [SerializeField] private TextMeshProUGUI _notifyText;
        // Start is called before the first frame update

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            bool result = bool.Parse(_customProperties["result"].ToString());
            _notifyText.text = result ? "You Win" : "You Close";
            StartCoroutine(ClosePopup());
        }

        IEnumerator ClosePopup() {
            yield return new WaitForSeconds(3f);
            Hide();
        }
    }
}
