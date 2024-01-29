using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HS4.Config;

namespace HS4.UI
{
    public class UIGemPack : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private Button _buyBtn;

        [SerializeField] private GameObject _loadingSpiner;
        private GemPack _gemPack;

        private bool _isPurchasing
        {
            set
            {
                _loadingSpiner.SetActive(value);
                _priceText.gameObject.SetActive(!value);
                _buyBtn.interactable = !value;
            }
        }

        private void Start()
        {
            _buyBtn.onClick.AddListener(ProcessBuy);
        }

        public void Setup(GemPack gemPack)
        {
            _gemPack = gemPack;
            _priceText.text = $"USD $<size=120%><#21bf82>{gemPack.Price.EN.ToString()}" ;
            _amountText.text = gemPack.Amount.ToString();
        }

        public async void ProcessBuy()
        {
            _isPurchasing = true;

            var purchaseResult = await User.Buy_Gem();

            if (purchaseResult.IsSuccess)
            {
                var rewardData = new RewardData
                {
                    Amount = int.Parse(purchaseResult.Data.ToString()),
                    RewardType = ERewardType.Gem
                };
                UIManager.Instance.ShowPopup(PopupName.Reward, new Dictionary<string, object>
                {
                    {"rewardData", rewardData }
                });

            }
            else
            {
                var notifyData = new NotifyData
                {
                    IsSuccess = false,
                    Title = "Purchase Fail",
                    Detail = purchaseResult.Message
                };
                UIManager.Instance.ShowPopup(PopupName.Notify, new Dictionary<string, object>
                {
                    { "notifyData", notifyData}
                });
            }

            _isPurchasing = false;
        }
    }

}
