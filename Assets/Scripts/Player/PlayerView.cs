using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using HS4.Config;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace HS4.PlayerCore
{
    public class PlayerView : NetworkBehaviour
    {
        [SerializeField] private TextMeshPro _playerNameText;
        [SerializeField] private PlayerAnimation _playerAnimation;
        [SerializeField] private List<SkinnedMeshRenderer> _skinMeshList = new();
        [SerializeField] private GameObject _caseOb;
        [SerializeField] private GameObject _targetBody;

        //Radar
        [SerializeField] private RadarView _radarView;
        [SerializeField] private CircleRadarView _circleRadarView;

        [SerializeField] private bool _isHider;
        //public NetworkVariable<bool> ObjectHide = new NetworkVariable<bool>(default,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public bool ObjectHide;
        public void SetCharacterType(bool isHider)
        {
            _isHider = isHider;
        }

        public void SetName(string name) 
        {
            if(string.IsNullOrEmpty(name))
                return;

            _playerNameText.text = name;
        }

        public void SetBody(string id)
        {
            if(string.IsNullOrEmpty(id))
                return;

            ObjectHide = false;

            if(_targetBody != null)
                Destroy(_targetBody);

            _targetBody = Instantiate(ConfigManager.Instance.GetCharacterPrefab(id), this.transform);
            _targetBody.transform.localPosition = Vector3.zero;
            _skinMeshList = _targetBody.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            _playerAnimation.SetAnimator(_targetBody.GetComponent<Animator>());
        }

        public void Reset()
        {
            _radarView.gameObject.SetActive(false);
            _circleRadarView.gameObject.SetActive(false);
            _playerAnimation.Idle();
            ObjectHide = false;
            ToggleSkin(true);
        }

        public void Hide()
        {
            ToggleSkin(false);
            ObjectHide = true;
        }

        private void ToggleSkin(bool value) {
            
            _playerNameText.gameObject.SetActive(value);
            foreach (var skin in _skinMeshList)
            {
                skin.enabled = value;
            }
        }

        public void TurnOnRadar()
        {
            _radarView.gameObject.SetActive(true);
            _circleRadarView.gameObject.SetActive(true);
        }

        public void TurnOnCircle() {
            _circleRadarView.gameObject.SetActive(true);
        }

        public void SetIsKill(bool isKill)
        {
            _caseOb.SetActive(isKill);
            if (isKill)
            {
                ToggleSkin(true);
                _playerAnimation.Die();
            }
        }


        private void FixedUpdate()
        {

            if (IsOwner && !_isHider)
            {
                var result = GetSeenVictim();
                if (result != null)
                {
                    GameController.Instance.KillPlayerServerRpc(result.GetComponent<NetworkObject>().OwnerClientId);
                }
            }
        }

        private Collider GetSeenVictim()
        {
            if (_isHider || !_radarView.gameObject.activeSelf || !_radarView.gameObject.activeSelf)
                return null;

            var result = _radarView.GetSeenVictim();
            return (null != result) ? result : _circleRadarView.GetSeenVictim();
        }

    }

}
