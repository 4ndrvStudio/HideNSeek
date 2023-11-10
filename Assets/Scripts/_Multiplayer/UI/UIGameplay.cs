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
        [SerializeField] private GameObject _startgamePanel;
        [SerializeField] private TextMeshProUGUI _timeText;
        [SerializeField] private TextMeshProUGUI _characterTypeText;

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

        public void DisplayGameStartUp(bool isHider) {
            _startgamePanel.SetActive(true);
            _characterTypeText.text = isHider ? "You are Hider!" : "You Are Seeker";
        }


    }

}
