using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HS4
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

        private AnimationType _animationType;

        public AnimationType AnimationType
        {
            get
            {
                return _animationType;
            }
        }

      public void SetAnimator(Animator animator)  => _animator = animator;

        public bool IsDied()
        {
            return _animationType == AnimationType.Died;
        }

        public virtual void Idle()
        {
            PlayAnimation(AnimationType.Idle);
        }

        public void Walk()
        {
            PlayAnimation(AnimationType.Walk);
        }

        public virtual void Die()
        {
            PlayAnimation(AnimationType.Died);
        }

        private void PlayAnimation(AnimationType animationType)
        {
            if (_animationType == animationType)
                return;

            _animationType = animationType;
            var nameHash = Animator.StringToHash(_animationType.ToString());

            _animator.Play(nameHash);

            if (_animator.HasState(1, nameHash))
            {
                _animator.Play(nameHash, 1);
            }
            _animator.Update(0);
        }
    }

}
