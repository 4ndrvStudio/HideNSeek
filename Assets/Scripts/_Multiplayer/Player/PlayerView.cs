using System.Collections;
using System.Collections.Generic;
using Game;
using Unity.Netcode;
using UnityEngine;

namespace HS4
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

        public void SetCharacterType(bool isHider)
        {
            _isHider = isHider;
            SetBody(isHider);
            SetRadar(isHider);
        }

        private void SetBody(bool isHider) {
            _hiderBody.SetActive(isHider);
            _seekerBody.SetActive(!isHider);
            var targetBody = isHider ? _hiderBody : _seekerBody;
            _skinMeshRender = targetBody.GetComponentInChildren<SkinnedMeshRenderer>();
            _playerAnimation.SetAnimator(targetBody.GetComponent<Animator>());
        }

        public void SetIsKill(bool isKill) {
            _caseOb.SetActive(isKill);
            if(isKill)
              _playerAnimation.IsDied();
        }

        private void SetRadar(bool isHider) {
            _radarView.gameObject.SetActive(!isHider);
            _circleRadarView.gameObject.SetActive(!isHider);
        }


        private void Update() {
            if(IsOwner && !_isHider) {
                var result = GetSeenVictim();
                if(result != null) {
                    GameController.Instance.KillPlayerServerRpc(result.GetComponent<NetworkObject>().OwnerClientId);
                }
            }
        }

        private Collider GetSeenVictim()
        {
            if (_isHider)
                return null;

            var result = _radarView.GetSeenVictim();
            return (null != result) ? result : _circleRadarView.GetSeenVictim();
        }

    }

}
