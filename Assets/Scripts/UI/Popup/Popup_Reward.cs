using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace HS4.UI
{
    public enum ERewardType
    {
        Gold,
        Gem
    }
    [System.Serializable]
    public class RewardConfig
    {
        public ERewardType RewardType;
        public Sprite Icon;
    }

    public class RewardData
    {
        public ERewardType RewardType;
        public int Amount;
    }

    public class Popup_Reward : UIPopup
    {
        [SerializeField] private Image _rewardIcon;
        [SerializeField] private TextMeshProUGUI _rewardAmountText;
        [SerializeField] private List<RewardConfig> _rewardConfigList = new();
        [SerializeField] private Button _okeBtn;

        public void Start()
        {
            _okeBtn.onClick.AddListener(() =>
            {
                UIManager.Instance.CurrentView.RefreshBalance();
                Hide();
            });
        }

        public override void Show(Dictionary<string, object> customProperties = null)
        {
            base.Show(customProperties);
            var rewardData = _customProperties["rewardData"] as RewardData;
            var rewardConfig = _rewardConfigList.Find(item => item.RewardType == rewardData.RewardType);

            _rewardAmountText.text = rewardData.Amount.ToString();
            _rewardIcon.sprite = rewardConfig.Icon;

        }
    }

}
