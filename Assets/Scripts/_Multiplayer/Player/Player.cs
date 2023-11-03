using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace HS4
{
    public class Player : NetworkBehaviour
    {
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerMovement _playerMovement;
        //stats
        [SerializeField] private NetworkVariable<bool> _isHider = new NetworkVariable<bool>(default,
                                                                    NetworkVariableReadPermission.Everyone,
                                                                    NetworkVariableWritePermission.Server);
        [SerializeField] private NetworkVariable<bool> _isKill = new NetworkVariable<bool>(default,
                                                                    NetworkVariableReadPermission.Everyone,
                                                                    NetworkVariableWritePermission.Server);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //if (!IsOwner)
            //{
            //    _playerCamera.Priority = 0;
            //    return;
            //}      
            _isHider.OnValueChanged += OnHiderStateChange;
            _isKill.OnValueChanged += OnIsKillStateChange;

            SetupCharacterType();

        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            _isHider.OnValueChanged -= OnHiderStateChange;
            _isKill.OnValueChanged -= OnIsKillStateChange;
        }

        public void SetIsHider() => _isHider.Value = true;

        public void SetIsKill()  => _isKill.Value = true;

        private void OnHiderStateChange(bool previous, bool current)
        {
            SetupCharacterType();
        }
        private void SetupCharacterType() {
            _playerView.SetCharacterType(_isHider.Value);
           if(_isHider.Value)  
                gameObject.layer =  LayerMask.NameToLayer("Victim");
        }

        private void OnIsKillStateChange(bool previous, bool current)
        {
             _playerView.SetIsKill(_isKill.Value);
            _playerMovement.SetCanMove(_isKill.Value);
             
        }

   

      
    }

}
