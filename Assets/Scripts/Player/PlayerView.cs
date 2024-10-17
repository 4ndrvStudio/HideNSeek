using System.Collections;
using System.Collections.Generic;
using Game;
using HS4.Backend;
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
        [SerializeField] private GameObject _playerNameText;

        [SerializeField] private List<GameObject> _bodyList = new();

        //Radar
        [SerializeField] private RadarView _radarView;
        [SerializeField] private CircleRadarView _circleRadarView;

        private GameObject Targetbody;

        [SerializeField] private bool _isHider;
        //public NetworkVariable<bool> ObjectHide = new NetworkVariable<bool>(default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public bool ObjectHide;
        public void SetCharacterType(bool isHider, string character_id)
        {
            _isHider = isHider;
            SetBody(isHider,character_id);
        }

        private void SetBody(bool isHider, string character_id) {
            // _hiderBody.SetActive(isHider);
            // _seekerBody.SetActive(!isHider);
            ObjectHide = false;
            char id = character_id[character_id.Length-1];
            _bodyList.ForEach(x => x.gameObject.SetActive(false));
            Targetbody = _bodyList[int.Parse(id.ToString()) -1];
            Targetbody.SetActive(true);
            _skinMeshRender = Targetbody.GetComponentInChildren<SkinnedMeshRenderer>();
            _playerAnimation.SetAnimator(Targetbody.GetComponent<Animator>());
        }

        public void Reset() {
            _radarView.gameObject.SetActive(false);
            _circleRadarView.gameObject.SetActive(false);
            _playerAnimation.Idle();
            ObjectHide = false;
        }

        public void Hide() {
            Targetbody.SetActive(false);
            _playerNameText.SetActive(false);
            ObjectHide = true;
        } 
    
        public void TurnOnRadar() {
            _radarView.gameObject.SetActive(true);
            _circleRadarView.gameObject.SetActive(true);
        }

        public void SetIsKill(bool isKill) {
            _caseOb.SetActive(isKill);
            if(isKill) {
                Targetbody.SetActive(true);
                 _playerNameText.SetActive(true);
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
