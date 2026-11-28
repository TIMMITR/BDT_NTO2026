using PrimeTween;
using UnityEngine;

namespace Features.ItemsPack.Hourglass
{
    public class HourglassView : MonoBehaviour
    {
        [SerializeField] private Transform glassTransform;
        [SerializeField] private Vector3 flipAxis = Vector3.right;
        [SerializeField] private float flipDuration = 0.6f;
        [SerializeField] private Ease flipEase = Ease.InOutSine;

        private Tween _flipAnimation;

        private void OnDestroy()
        {
            if (_flipAnimation.isAlive)
            {
                _flipAnimation.Stop();
            }
        }

        public void PlayFlip()
        {
            if (_flipAnimation.isAlive)
            {
                return;
            }

            var startRotation = glassTransform.eulerAngles;
            var targetRotation = startRotation + flipAxis.normalized * 180f;
            _flipAnimation = Tween.EulerAngles(
                glassTransform,
                startRotation,
                targetRotation,
                flipDuration,
                flipEase);
        }
    }
}
