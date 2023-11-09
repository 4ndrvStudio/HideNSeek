using System.Collections;
using System.Collections.Generic;
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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //if (!IsOwner)
            //{
            //    _playerCamera.Priority = 0;
            //    return;
            //}      
            if(IsOwner) {
                if(LocalPlayer == null) {
                    LocalPlayer =this;
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
            if(IsServer) {
                PlayerMovement.SetCanMove(IsKill.Value);
            }
            
            PlayerView.SetIsKill(IsKill.Value);
          
        }


   

      
    }

}
