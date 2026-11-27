using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public class PickUnitView : MonoBehaviour
    {
        public Button pickUnitButton;
        public RectTransform statsPanel;

        [SerializeField] private Text healthText;
        [SerializeField] private Text defenceText;
        [SerializeField] private Text speedText;
        [SerializeField] private RectTransform openStatsButton;
        [SerializeField] private float openDuration = 0.2f;
        [SerializeField] private float closeDuration = 0.2f;
        [SerializeField] private float panelSlideDistance = 24f;
        [SerializeField] private float buttonSlideDistance = 14f;
        [SerializeField] private float collapsedWidth = 0.15f;

        private CanvasGroup _statsCanvasGroup;
        private CanvasGroup _openButtonCanvasGroup;
        private Vector2 _statsPosition;
        private Vector2 _openButtonPosition;
        private Vector3 _statsScale;
        private Vector3 _openButtonScale;
        private Sequence _animation;
        private bool _isOpen;
        private bool _isInitialized;

        private void Awake()
        {
            Initialize();
            _isOpen = statsPanel.gameObject.activeSelf;
            ApplyStateImmediately(_isOpen);
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        public void UpdateStats(
            int currentHealth,
            int maxHealth,
            int defence,
            int currentSpeed,
            int maxSpeed)
        {
            healthText.text = $"{currentHealth}/{maxHealth}";
            defenceText.text = defence.ToString();
            speedText.text = $"{currentSpeed}/{maxSpeed}";
        }

        public void ClearStats()
        {
            healthText.text = "-";
            defenceText.text = "-";
            speedText.text = "-";
        }

        public void OpenStats()
        {
            if (_isOpen) return;

            _isOpen = true;
            AnimateState(true);
        }

        public void CloseStats()
        {
            if (!_isOpen) return;

            _isOpen = false;
            AnimateState(false);
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            _statsPosition = statsPanel.anchoredPosition;
            _statsScale = statsPanel.localScale;
            _openButtonPosition = openStatsButton.anchoredPosition;
            _openButtonScale = openStatsButton.localScale;
            _statsCanvasGroup = GetCanvasGroup(statsPanel);
            _openButtonCanvasGroup = GetCanvasGroup(openStatsButton);
            _isInitialized = true;
        }

        private void AnimateState(bool open)
        {
            StopAnimation();

            statsPanel.gameObject.SetActive(true);
            openStatsButton.gameObject.SetActive(true);
            _statsCanvasGroup.blocksRaycasts = open;
            _openButtonCanvasGroup.blocksRaycasts = false;

            var statsTargetPosition = _statsPosition + (open ? Vector2.zero : Vector2.left * panelSlideDistance);
            var statsTargetScale = open ? _statsScale : GetCollapsedStatsScale();
            var buttonTargetPosition = _openButtonPosition + (open ? Vector2.left * buttonSlideDistance : Vector2.zero);
            var buttonTargetScale = open ? _openButtonScale * 0.9f : _openButtonScale;
            var duration = open ? openDuration : closeDuration;
            var positionEase = open ? Ease.OutExpo : Ease.InExpo;
            var scaleEase = open ? Ease.OutExpo : Ease.InExpo;

            _animation = Sequence.Create(useUnscaledTime: true)
                .Group(Tween.UIAnchoredPosition(
                    statsPanel,
                    statsPanel.anchoredPosition,
                    statsTargetPosition,
                    duration,
                    positionEase))
                .Group(Tween.Scale(
                    statsPanel,
                    statsPanel.localScale,
                    statsTargetScale,
                    duration,
                    scaleEase))
                .Group(Tween.Alpha(
                    _statsCanvasGroup,
                    _statsCanvasGroup.alpha,
                    open ? 1f : 0f,
                    duration,
                    open ? Ease.OutQuad : Ease.InQuad))
                .Group(Tween.UIAnchoredPosition(
                    openStatsButton,
                    openStatsButton.anchoredPosition,
                    buttonTargetPosition,
                    duration,
                    positionEase))
                .Group(Tween.Scale(
                    openStatsButton,
                    openStatsButton.localScale,
                    buttonTargetScale,
                    duration,
                    open ? Ease.InOutQuad : Ease.OutBack))
                .Group(Tween.Alpha(
                    _openButtonCanvasGroup,
                    _openButtonCanvasGroup.alpha,
                    open ? 0f : 1f,
                    duration,
                    open ? Ease.InQuad : Ease.OutQuad))
                .ChainCallback(this, view => view.FinishAnimation(open));
        }

        private void FinishAnimation(bool open)
        {
            _animation = default;
            statsPanel.gameObject.SetActive(open);
            openStatsButton.gameObject.SetActive(!open);
            _statsCanvasGroup.blocksRaycasts = open;
            _openButtonCanvasGroup.blocksRaycasts = !open;
        }

        private void ApplyStateImmediately(bool open)
        {
            statsPanel.anchoredPosition = _statsPosition + (open ? Vector2.zero : Vector2.left * panelSlideDistance);
            statsPanel.localScale = open ? _statsScale : GetCollapsedStatsScale();
            openStatsButton.anchoredPosition = _openButtonPosition + (open ? Vector2.left * buttonSlideDistance : Vector2.zero);
            openStatsButton.localScale = open ? _openButtonScale * 0.9f : _openButtonScale;
            _statsCanvasGroup.alpha = open ? 1f : 0f;
            _openButtonCanvasGroup.alpha = open ? 0f : 1f;
            statsPanel.gameObject.SetActive(open);
            openStatsButton.gameObject.SetActive(!open);
            _statsCanvasGroup.blocksRaycasts = open;
            _openButtonCanvasGroup.blocksRaycasts = !open;
        }

        private Vector3 GetCollapsedStatsScale()
        {
            return Vector3.Scale(_statsScale, new Vector3(collapsedWidth, 0.92f, 1f));
        }

        private void StopAnimation()
        {
            if (_animation.isAlive)
            {
                _animation.Stop();
            }
            _animation = default;
        }

        private static CanvasGroup GetCanvasGroup(Component target)
        {
            var canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.gameObject.AddComponent<CanvasGroup>();
            }
            return canvasGroup;
        }
    }
}
