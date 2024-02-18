using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace HS4.PlayerCore
{
    public class Player : NetworkBehaviour
    {
        public static Player LocalPlayer;
        [Header("Refs")]
        public PlayerView PlayerView;
        public PlayerMovement PlayerMovement;
        //stats
        public NetworkVariable<bool> IsHider = new NetworkVariable<bool>(true);
        public NetworkVariable<bool> IsKill = new NetworkVariable<bool>(false);

        private Vector3 _rootPosition;
        private Quaternion _rootRotation;

        [Header("Info")]
        [SerializeField] private TextMeshPro _playerNameText;
        [SerializeField] private List<GameObject> _playerBody; 
        //network
        public NetworkVariable<string> PlayerName = new NetworkVariable<string>(default);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _rootPosition = transform.position;
            _rootRotation = transform.rotation;
            
            if(IsOwner) {
                if(LocalPlayer == null) {
                    LocalPlayer =this;
                    
                    //setname 

                    //set body 
                    
                }
            }


            IsHider.OnValueChanged += OnHiderStateChange;
            IsKill.OnValueChanged += OnIsKillStateChange;

            if(IsServer) {
                IsHider.Value = true;
            }

            SetupCharacterType();
           
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            IsHider.OnValueChanged -= OnHiderStateChange;
            IsKill.OnValueChanged -= OnIsKillStateChange;
            LocalPlayer = null;
        }

        [ClientRpc]
        public void SetStartHideAndSeekClientRpc() 
        {
            if(IsOwner) 
            {
                if(IsHider.Value == false) {
                    PlayerView.TurnOnRadar();
                } 
            } else {
                if(LocalPlayer.IsHider.Value == false && IsHider.Value == true) {
                   PlayerView.Hide();
                }
            }
        }

        public async void Reset() {
            IsHider.Value = true;
            await Task.Delay(2000);
            IsKill.Value = false;
            ResetPositionAndViewClientRpc();
        }

        [ClientRpc]
        public void ResetPositionAndViewClientRpc() {
            if(IsOwner || IsServer) {
                transform.position = _rootPosition;
                transform.rotation = _rootRotation;
            }
            
            PlayerView.Reset();
            PlayerMovement.ResetPlayer();
        }

        public void SetIsSeeker() 
        {
            if(IsServer) {
                 IsHider.Value = false;
            }
        } 

        [ClientRpc]
        public void StartMoveClientRpc() 
        {
            PlayerMovement.EnableInput();
        }
        
        public void SetIsKill() 
        {
            if(IsServer)
                IsKill.Value = true;
        } 

        private void OnHiderStateChange(bool previous, bool current)
        {
            SetupCharacterType();
        }
        private void SetupCharacterType() 
        {
           PlayerView.SetCharacterType(IsHider.Value);
            
           if(IsHider.Value)  
                gameObject.layer =  LayerMask.NameToLayer("Victim");
            else 
                gameObject.layer =  default;
        }

        private void OnIsKillStateChange(bool previous, bool current)
        {
            PlayerMovement.SetCanMove(IsKill.Value);
            PlayerView.SetIsKill(IsKill.Value);
        }

        private void OnNameChange(string prev, string current) {
            _playerNameText.text = current;
        }

        private void OnBodyChange(string prevId, string currentId) {
            
        }
        


   

      
    }

}
