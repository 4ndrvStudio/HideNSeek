using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace HS4.UI
{
    public class UIGameplay : MonoBehaviour
    {
        public static UIGameplay Instance;
        [SerializeField] private GameObject _startupPanel;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _characterTypeText;

        public Image TimeReminingIcon;
        [SerializeField] private GameObject _gameplayPanel;
        [SerializeField] private GameObject _targetHolder;
        [SerializeField] private GameObject _targetSlot;
        [SerializeField] private List<GameObject> _targetList = new();

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void UpdateTime(string time)
        {
            _timeText.text = time;
        }

        public void DisplayGameStartUp(bool isHider)
        {
            _startupPanel.SetActive(true);
            _characterTypeText.text = isHider ? "You are Hider!" : "You Are Seeker";
        }

        public void DisplayInPlayGame(Dictionary<ulong, PlayerInRoom> playerList)
        {
            _startupPanel.SetActive(false);
            _gameplayPanel.SetActive(true);
            UpdateTargetList(playerList);
        }

        public void HideInPlayGameUI() {
            _startupPanel.SetActive(false);
             _gameplayPanel.SetActive(false);
        }

        public void UpdateTargetList(Dictionary<ulong, PlayerInRoom> playerList)
        {
            _targetList.ForEach(item => Destroy(item));
            _targetList.Clear();

            foreach (var player in playerList.Values)
            {
                if (player.IsHider)
                {
                    GameObject targetSlot = Instantiate(_targetSlot, _targetHolder.transform);
                    targetSlot.SetActive(true);
                    targetSlot.transform.GetChild(0).gameObject.SetActive(player.WasCatching);
                    _targetList.Add(targetSlot);
                }
            }
        }




    }

}
