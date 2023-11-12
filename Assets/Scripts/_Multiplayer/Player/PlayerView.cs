using System.Collections;
using System.Collections.Generic;
using Game;
using Unity.Netcode;
using UnityEngine;

namespace HS4.PlayerCore
{
    public class PlayerView : NetworkBehaviour
    {

        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private SkinnedMeshRenderer _skinMeshRender;
        [SerializeField] private GameObject _hiderBody;
        [SerializeField] private GameObject _seekerBody;
        [SerializeField] private GameObject _caseOb;
        //Radar

        [SerializeField] private RadarView _radarView;
        [SerializeField] private CircleRadarView _circleRadarView;

        [SerializeField] private bool _isHider;
        //public NetworkVariable<bool> ObjectHide = new NetworkVariable<bool>(default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public bool ObjectHide;
        public void SetCharacterType(bool isHider)
        {
            _isHider = isHider;
            SetBody(isHider);
        }

        private void SetBody(bool isHider) {
            _hiderBody.SetActive(isHider);
            _seekerBody.SetActive(!isHider);
            ObjectHide = false;
            var targetBody = isHider ? _hiderBody : _seekerBody;
            _skinMeshRender = targetBody.GetComponentInChildren<SkinnedMeshRenderer>();
            _playerAnimation.SetAnimator(targetBody.GetComponent<Animator>());
        }

        public void Reset() {
            _radarView.gameObject.SetActive(false);
            _circleRadarView.gameObject.SetActive(false);
            _playerAnimation.Idle();
            ObjectHide = false;
        }

        public void Hide() {
            _skinMeshRender.enabled = false;
            ObjectHide = true;
        } 
    
        public void TurnOnRadar() {
            _radarView.gameObject.SetActive(true);
            _circleRadarView.gameObject.SetActive(true);
        }

        public void SetIsKill(bool isKill) {
            _caseOb.SetActive(isKill);
            if(isKill) {
                _skinMeshRender.enabled = true;
                _playerAnimation.Die();
            } 
        }


        private void FixedUpdate() {

            if(IsOwner && !_isHider) {
                var result = GetSeenVictim();
                if(result != null) {
                    GameController.Instance.KillPlayerServerRpc(result.GetComponent<NetworkObject>().OwnerClientId);
                }
            }
        }

        private Collider GetSeenVictim()
        {
            if (_isHider || !_radarView.gameObject.activeSelf ||!_radarView.gameObject.activeSelf)
                return null;

            var result = _radarView.GetSeenVictim();
            return (null != result) ? result : _circleRadarView.GetSeenVictim();
        }

    }

}
