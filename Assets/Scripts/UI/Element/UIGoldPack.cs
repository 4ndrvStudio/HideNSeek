using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HS4.Config;

namespace HS4.UI
{
    public class UIGoldPack : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _amountText;
        [SerializeField] private TextMeshProUGUI _priceText;
        [SerializeField] private GameObject _loadingSpiner;
        [SerializeField] private Button _buyBtn;
        private GoldPack _goldPack;

        private bool _isPurchasing
        {
            set
            {
                _loadingSpiner.SetActive(value);
                _buyBtn.interactable = !value;
            }
        }

        private void Start()
        {
            _buyBtn.onClick.AddListener(ProcessBuy);
        }

        public void Setup(GoldPack goldPack)
        {
            _goldPack = goldPack;
            _priceText.text = goldPack.Price.ToString();
            _amountText.text = goldPack.Amount.ToString();
        }

        public async void ProcessBuy()
        {
            _isPurchasing = true;

            var purchaseResult = await User.Buy_Gold(_goldPack.PackId);

            if(purchaseResult.IsSuccess)
            {
                var rewardData = new RewardData
                {
                    Amount = int.Parse(purchaseResult.Data.ToString()),
                    RewardType = ERewardType.Gold
                };
                UIManager.Instance.ShowPopup(PopupName.Reward,new Dictionary<string, object>
                {
                    {"rewardData", rewardData }
                });

            } else
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