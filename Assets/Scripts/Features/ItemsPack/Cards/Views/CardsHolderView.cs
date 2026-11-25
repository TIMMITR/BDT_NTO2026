using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Features.ItemsPack.Cards.Views
{
    public class CardsHolderView : MonoBehaviour
    {
        public Button cardsButton;
        public Image image;
        public Text cardName;
        public Text cardDescription;
        
        [SerializeField] private float appearDuration = 0.45f;
        [SerializeField] private float moveToCenterDuration = 0.45f;
        [SerializeField] private float disappearDuration = 0.35f;
        [SerializeField] private float selectedHoldDuration = 3f;
        [SerializeField] private float appearDelayStep = 0.08f;
        [SerializeField] private float offscreenDistance = 1000f;
        [SerializeField] private float selectedScale = 1.12f;

        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _idlePosition;
        private Vector3 _idleScale;
        private Sequence _animation;
        private bool _isInitialized;

        private Sprite _sprite;

        private void Awake()
        {
            _sprite = image.sprite;
            Initialize();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        public void TurnCard(string cName, string description, Sprite sprite)
        {
            if (cardName is null)
            {
                Debug.LogWarning("Card name is null");
                return;
            }
            cardName.text = cName;
            image.sprite = sprite;
            cardDescription.text = description;
        }

        public void Show(int cardIndex)
        {
            Initialize();
            StopAnimation();

            gameObject.SetActive(true);
            cardsButton.interactable = true;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            var delay = cardIndex * appearDelayStep;
            var hiddenPosition = _idlePosition + Vector2.down * offscreenDistance;
            var startScale = _idleScale * 0.85f;
            _rectTransform.anchoredPosition = hiddenPosition;
            transform.localScale = startScale;

            _animation = Sequence.Create()
                .Group(Tween.UIAnchoredPosition(
                    _rectTransform,
                    hiddenPosition,
                    _idlePosition,
                    appearDuration,
                    Ease.OutCubic,
                    startDelay: delay))
                .Group(Tween.Scale(
                    transform,
                    startScale,
                    _idleScale,
                    appearDuration,
                    Ease.OutBack,
                    startDelay: delay));
        }

        public void PlaySelected(Vector2 centerPosition)
        {
            Initialize();
            StopAnimation();

            cardsButton.interactable = true;
            _canvasGroup.blocksRaycasts = false;

            var currentPosition = _rectTransform.anchoredPosition;
            var currentScale = transform.localScale;
            var enlargedScale = _idleScale * selectedScale;
            var exitPosition = centerPosition + Vector2.up * offscreenDistance;

            _animation = Sequence.Create()
                .Group(Tween.UIAnchoredPosition(
                    _rectTransform,
                    currentPosition,
                    centerPosition,
                    moveToCenterDuration,
                    Ease.OutBack))
                .Group(Tween.Scale(
                    transform,
                    currentScale,
                    enlargedScale,
                    moveToCenterDuration,
                    Ease.OutBack))
                .ChainDelay(selectedHoldDuration)
                .Chain(Tween.UIAnchoredPosition(
                    _rectTransform,
                    centerPosition,
                    exitPosition,
                    disappearDuration,
                    Ease.InCubic))
                .Group(Tween.Scale(
                    transform,
                    enlargedScale,
                    _idleScale * 0.8f,
                    disappearDuration,
                    Ease.InQuad))
                .ChainCallback(this, view => view.Deactivate());
        }

        public void Hide()
        {
            cardsButton.interactable = false;
            if (!gameObject.activeSelf) return;

            Initialize();
            StopAnimation();
            _canvasGroup.blocksRaycasts = false;

            var currentPosition = _rectTransform.anchoredPosition;
            var currentScale = transform.localScale;
            var exitPosition = _idlePosition + Vector2.down * offscreenDistance;

            _animation = Sequence.Create()
                .Group(Tween.UIAnchoredPosition(
                    _rectTransform,
                    currentPosition,
                    exitPosition,
                    disappearDuration,
                    Ease.InCubic))
                .Group(Tween.Scale(
                    transform,
                    currentScale,
                    _idleScale * 0.8f,
                    disappearDuration,
                    Ease.InQuad))
                .ChainCallback(this, view => view.Deactivate());
        }

        public void HideImmediately()
        {
            Initialize();
            StopAnimation();
            Deactivate();
        }

        private void Initialize()
        {
            if (_isInitialized) return;

            _rectTransform = (RectTransform)transform;
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            _idlePosition = _rectTransform.anchoredPosition;
            _idleScale = transform.localScale;
            _isInitialized = true;
        }

        private void StopAnimation()
        {
            if (_animation.isAlive)
            {
                _animation.Stop();
            }
            _animation = default;
        }

        private void Deactivate()
        {
            _animation = default;
            _rectTransform.anchoredPosition = _idlePosition;
            transform.localScale = _idleScale;
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            cardsButton.interactable = false;
            image.sprite = _sprite;
            cardName.text = string.Empty;
            cardDescription.text = string.Empty;
            gameObject.SetActive(false);
        }
    }
}
