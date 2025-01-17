﻿using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace HS4
{
    public class NetworkStartUI : MonoBehaviour {
 
        [SerializeField] private Button _startClientButton;
        [SerializeField] private Button _startHostButton;
        [SerializeField] private TMP_InputField _joinCodeInput;
        
        void Start() {
            _startHostButton.onClick.AddListener(StartHost);
            _startClientButton.onClick.AddListener(StartClient);
            Application.targetFrameRate = 60;
        }

        async void StartClient() {
            if(RelayManager.Instance.IsRelayEnabled) {
                await RelayManager.Instance.JoinRelay(_joinCodeInput.text);
            }
            
            NetworkManager.Singleton.StartClient();
            
            Hide();
        }

         async void StartHost() 
         {
            if(RelayManager.Instance.IsRelayEnabled) {
                await RelayManager.Instance.SetupHost(6);
            }
            
            NetworkManager.Singleton.StartHost();
            Hide();
        }

        void Hide() => gameObject.SetActive(false);
    }
}