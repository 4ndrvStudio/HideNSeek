using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

namespace HS4.PlayerCore
{
    public enum AnimationType
    {
        Idle,
        Walk,
        Died
    }

    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private RuntimeAnimatorController _controller;

        private AnimationType _animationType;

        public AnimationType AnimationType
        {
            get
            {
                return _animationType;
            }
        }

      public void SetAnimator(Animator animator) {
            _animator = animator;
            _animator.runtimeAnimatorController = _controller;
      }  

        public bool IsDied()
        {
            return _animationType == AnimationType.Died;
        }

        public virtual void Idle()
        {
            _animator.SetBool("isWalk",false);
            _animator.SetBool("isDeath",false);
        }

        public void Walk()
        {
            _animator.SetBool("isWalk",true);
        }

        public virtual void Die()
        {
            _animator.SetBool("isWalk",false);
            _animator.SetBool("isDeath",true);

        }

    
    }

}
