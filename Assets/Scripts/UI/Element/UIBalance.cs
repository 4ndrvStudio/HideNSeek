using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace HS4.UI
{
    public class UIBalance : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _energyText;
        [SerializeField] private TextMeshProUGUI _goldText;
        [SerializeField] private TextMeshProUGUI _gemText;

        // Start is called before the first frame update
        void Start()
        {

        }

        public async void UpdateBalance()
        {
            GetBalance();
            await User.GetBalance();
            GetBalance();
        }

        private void GetBalance()
        {
            _energyText.text = User.Balance.Energy.ToString();
            _goldText.text = User.Balance.Gold.ToString();
            _gemText.text = User.Balance.Gem.ToString();
        }
    }

}
