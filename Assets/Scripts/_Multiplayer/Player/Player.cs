using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace HS4
{
    public class Player : NetworkBehaviour
    {
        [Header("Refs")]
        public PlayerView PlayerView;
        [SerializeField] private PlayerMovement _playerMovement;
        //stats
        public  NetworkVariable<bool> IsHider = new NetworkVariable<bool>(false);
       public NetworkVariable<bool> IsKill = new NetworkVariable<bool>(false);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //if (!IsOwner)
            //{
            //    _playerCamera.Priority = 0;
            //    return;
            //}      
            IsHider.OnValueChanged += OnHiderStateChange;
            IsKill.OnValueChanged += OnIsKillStateChange;

            SetupCharacterType();

        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            IsHider.OnValueChanged -= OnHiderStateChange;
            IsKill.OnValueChanged -= OnIsKillStateChange;
        }

        [ClientRpc]
        public void SetStartHideAndSeekClientRpc() {
            if(IsOwner) {
                GameController.Instance.HideEnemy(IsHider.Value);
            }
        }

        public void SetIsHider() => IsHider.Value = true;

        public void SetIsKill()  => IsKill.Value = true;

        private void OnHiderStateChange(bool previous, bool current)
        {
            SetupCharacterType();
        }
        private void SetupCharacterType() 
        {
           PlayerView.SetCharacterType(IsHider.Value);
         
           if(IsHider.Value)  
                gameObject.layer =  LayerMask.NameToLayer("Victim");
        }

        private void OnIsKillStateChange(bool previous, bool current)
        {
            PlayerView.SetIsKill(IsKill.Value);
            _playerMovement.SetCanMove(IsKill.Value);
        }


   

      
    }

}
