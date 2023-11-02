using System;
using Game.Enemy;
using Game.Utilities;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Game
{
    public sealed class UnitView : AnimationUnitView
    {
        public Action<Node> NodeEntered;

        [SerializeField]
        private RadarView _radar;
        [SerializeField]
        private CircleRadarView _circleRadar;
        [SerializeField]
        private Animator _enemyBehaviour;
        [SerializeField]
        private SkinnedMeshRenderer _meshRenderer;
        [SerializeField]
        private CapsuleCollider _capsuleCollider;
        [SerializeField]
        public NavMeshAgent MeshAgent;

        private bool _isVictim;
        private bool _isVissible;

        public bool IsVictim
        {
            get
            {
                return _isVictim;
            }
            set
            {
                _isVictim = value;

                gameObject.layer = LayerUtils.GetVictimLayer(value);

                _radar.gameObject.SetActive(!value);
                _circleRadar.gameObject.SetActive(!value);
            }
        }

        public bool IsVissible
        {
            get
            {
                return _isVissible;
            }
            set
            {
                _isVissible = value;
                _meshRenderer.enabled = value;
            }
        }

        public bool IsAI
        {
            set
            {
                _enemyBehaviour.enabled = value;
            }
        }

        public float Radius
        {
            get
            {
                return _capsuleCollider.radius;
            }
        }

        private void Awake()
        {
            _enemyBehaviour.enabled = false;
        }

        public override void Die()
        {
            base.Die();
            _meshRenderer.enabled = true;
            _enemyBehaviour.enabled = false;
            IsVictim = false;

            _radar.gameObject.SetActive(false);
            _circleRadar.gameObject.SetActive(false);
        }

        public void FireNodeEnter(Node node)
        {
            NodeEntered.SafeInvoke(node);
        }

        public void RestartNodes()
        {
            _enemyBehaviour.Rebind();
            _enemyBehaviour.speed = 0;
        }

        public Collider GetSeenVictim()
        {
            if (_isVictim)
                return null;

            var result = _radar.GetSeenVictim();
            return (null != result) ? result : _circleRadar.GetSeenVictim();
        }

        public void SetOutline(Material[] materials)
        {
            Material[] result = new Material[] { _meshRenderer.sharedMaterial };

            if (null != materials)
            {
                ArrayUtils.AddRange(ref result, materials.Length, materials);
            }
            _meshRenderer.materials = result;
        }
    }
}