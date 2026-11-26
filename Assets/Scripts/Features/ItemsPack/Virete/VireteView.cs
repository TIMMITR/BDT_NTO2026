using PrimeTween;
using UnityEngine;

namespace Features.ItemsPack.Virete
{
    public class VireteView : MonoBehaviour
    {
        public Transform head;

        [SerializeField] private Vector3 talkingFaceOffset = new(0f, 180f, 0f);
        [SerializeField] private float faceTurnDuration = 0.45f;
        [SerializeField] private float boredTurnDuration = 0.65f;
        [SerializeField] private Ease faceTurnEase = Ease.InOutSine;
        [SerializeField] private Ease boredTurnEase = Ease.InOutSine;

        private Vector3 _idleHeadRotation;
        private Sequence _animation;
        private bool _isTalking;
        private bool _isInitialized;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        public void SetTalking(bool value)
        {
            if (!Initialize() || _isTalking == value) return;

            _isTalking = value;
            AnimateToCurrentFace();
        }

        public void PlayBored()
        {
            if (!Initialize()) return;

            StopAnimation();

            var startRotation = head.localEulerAngles;
            var firstTurnEnd = startRotation + Vector3.up * 360f;
            var targetRotation = GetTargetRotation();

            _animation = Sequence.Create()
                .Chain(Tween.LocalEulerAngles(
                    head,
                    startRotation,
                    firstTurnEnd,
                    boredTurnDuration,
                    boredTurnEase))
                .Chain(Tween.LocalEulerAngles(
                    head,
                    firstTurnEnd,
                    startRotation,
                    boredTurnDuration,
                    boredTurnEase))
                .Chain(Tween.LocalEulerAngles(
                    head,
                    startRotation,
                    targetRotation,
                    faceTurnDuration,
                    faceTurnEase))
                .ChainCallback(this, view => view.FinishAnimation());
        }

        private bool Initialize()
        {
            if (_isInitialized) return head != null;
            
            if (head == null) return false;

            _idleHeadRotation = head.localEulerAngles;
            _isInitialized = true;
            return true;
        }

        private void AnimateToCurrentFace()
        {
            StopAnimation();

            _animation = Sequence.Create()
                .Group(Tween.LocalEulerAngles(
                    head,
                    head.localEulerAngles,
                    GetTargetRotation(),
                    faceTurnDuration,
                    faceTurnEase))
                .ChainCallback(this, view => view.FinishAnimation());
        }

        private Vector3 GetTargetRotation()
        {
            return _idleHeadRotation + (_isTalking ? talkingFaceOffset : Vector3.zero);
        }

        private void FinishAnimation()
        {
            _animation = default;
        }

        private void StopAnimation()
        {
            if (_animation.isAlive)
            {
                _animation.Stop();
            }
            _animation = default;
        }
    }
}
