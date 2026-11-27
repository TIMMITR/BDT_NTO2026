using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class UnitHoverPanelView : MonoBehaviour
    {
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private RectTransform panel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text nameText;
        [SerializeField] private Text hpText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text defenceText;
        [SerializeField] private Vector3 worldOffset = new(0f, 0.5f, 0f);
        [SerializeField] private float animationDuration = 0.1f;

        private Tween _animation;
        private bool _isHiding;

        private void Awake()
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        public void Show(
            string unitName,
            int currentHealth,
            int maxHealth,
            int defence,
            int currentSpeed,
            int maxSpeed)
        {
            nameText.text = unitName;
            
            hpText.text = $"{currentHealth}/{maxHealth}";
            speedText.text = $"{currentSpeed}/{maxSpeed}";
            defenceText.text = defence.ToString();
            
            if (gameObject.activeSelf && !_isHiding)
                return;

            StopAnimation();
            _isHiding = false;
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                canvasGroup.alpha = 0f;
            }

            _animation = Tween.Alpha(canvasGroup, 1f, animationDuration, Ease.OutQuad);
        }

        public void SetWorldPosition(Camera mainCamera, Vector3 worldPosition)
        {
            var screenPosition = mainCamera.WorldToScreenPoint(worldPosition + worldOffset);
            if (screenPosition.z < 0f)
                return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    screenPosition,
                    null,
                    out var localPosition))
            {
                panel.anchoredPosition = localPosition;
            }
        }

        public void Hide()
        {
            if (!gameObject.activeSelf || _isHiding)
                return;

            StopAnimation();
            _isHiding = true;
            _animation = Tween.Alpha(canvasGroup, 0f, animationDuration, Ease.InQuad)
                .OnComplete(this, view => view.Deactivate());
        }

        private void Deactivate()
        {
            _animation = default;
            _isHiding = false;
            gameObject.SetActive(false);
        }

        private void StopAnimation()
        {
            if (_animation.isAlive)
                _animation.Stop();
            _animation = default;
        }

        private void OnDisable()
        {
            StopAnimation();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }
    }
}
