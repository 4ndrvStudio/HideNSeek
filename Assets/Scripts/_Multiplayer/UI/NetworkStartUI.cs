using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace HS4
{
    public class NetworkStartUI : MonoBehaviour {
 
        [SerializeField] private Button _startClientButton;
        [SerializeField] private Button _startHostButton;
        
        void Start() {
            _startHostButton.onClick.AddListener(StartHost);
            _startClientButton.onClick.AddListener(StartClient);
        }

        void StartClient() {
            NetworkManager.Singleton.StartClient();
            Hide();
        }

        void StartHost() {
            NetworkManager.Singleton.StartHost();
            Hide();
        }

        void Hide() => gameObject.SetActive(false);
    }
}