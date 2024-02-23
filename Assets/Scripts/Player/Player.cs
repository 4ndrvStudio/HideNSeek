using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
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
        //network
        public NetworkVariable<FixedString64Bytes> PlayerName = new NetworkVariable<FixedString64Bytes>
                                                                    (default,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        public NetworkVariable<FixedString64Bytes> PlayerBodyId = new NetworkVariable<FixedString64Bytes>
                                                                    (default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if(IsOwner) 
            {
                //get data
                if(LocalPlayer == null) 
                    LocalPlayer =this;

                GetInfoData();
            }

            InitializeData();          

            //event
            IsHider.OnValueChanged += OnHiderStateChange;
            IsKill.OnValueChanged += OnIsKillStateChange;
            PlayerName.OnValueChanged += OnNameChange;
            PlayerBodyId.OnValueChanged += OnBodyChange;
           
        }

        public void InitializeData() 
        {

            if(IsServer) {
                IsHider.Value = true;
            }
            SetupCharacterType();
            PlayerView.SetBody(PlayerBodyId.Value.ToString());  
            PlayerView.SetName(PlayerName.Value.ToString());

            //root pos
            _rootPosition = transform.position;
            _rootRotation = transform.rotation;
        }

        public async void GetInfoData() 
        {
            var idResult = await User.GetCharacterInUse(); 
            string characterId =  idResult.IsSuccess ? idResult.Data.ToString() : "character_1";
            PlayerBodyId.Value = characterId;
            PlayerName.Value = User.Info.UserName;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            IsHider.OnValueChanged -= OnHiderStateChange;
            IsKill.OnValueChanged -= OnIsKillStateChange;
            PlayerBodyId.OnValueChanged -= OnBodyChange;
            PlayerName.OnValueChanged -= OnNameChange;
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
                } else {
                   PlayerView.TurnOnCircle();
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
        public void ResetPositionAndViewClientRpc() 
        {
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

        private void OnNameChange(FixedString64Bytes prev, FixedString64Bytes current) 
        {
            PlayerView.SetName(current.ToString());
        }

        private void OnBodyChange(FixedString64Bytes prevId, FixedString64Bytes currentId) 
        {
            PlayerView.SetBody(currentId.ToString());  
        }

    
        


   

      
    }

}
