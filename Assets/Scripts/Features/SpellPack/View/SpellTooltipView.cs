using System.Text;
using Configs;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace Features.SpellPack.View
{
    public sealed class SpellTooltipView : MonoBehaviour
    {
        [SerializeField] private RectTransform panel;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Text nameText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text damageText;
        [SerializeField] private Text rangeText;
        [SerializeField] private Text radiusText;
        [SerializeField] private Text manaCostText;
        [SerializeField] private float animationDuration = 0.15f;

        private Sequence _animation;

        private void Awake()
        {
            HideImmediately();
        }

        public void Show(SpellConfigSo spell)
        {
            if (spell == null)
            {
                Hide();
                return;
            }
            StopAnimation();
            gameObject.SetActive(true);

            nameText.text = spell.DisplayName;
            descriptionText.text = spell.Description;
            damageText.text = spell.damage.ToString();
            rangeText.text = spell.range.ToString();
            radiusText.text = spell.areaRadius.ToString();
            manaCostText.text = spell.manaCost.ToString();

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            panel.localScale = Vector3.one * 0.96f;

            _animation = Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Alpha(canvasGroup, 1f, animationDuration, Ease.OutQuad))
                .Group(Tween.Scale(panel, Vector3.one, animationDuration, Ease.OutBack));
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            StopAnimation();
            _animation = Sequence.Create(useUnscaledTime: true)
                .Group(Tween.Alpha(canvasGroup, 0f, animationDuration, Ease.InQuad))
                .Group(Tween.Scale(panel, Vector3.one * 0.96f, animationDuration, Ease.InQuad))
                .ChainCallback(this, view => view.Deactivate());
        }

        public void HideImmediately()
        {
            StopAnimation();
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        private void Deactivate()
        {
            _animation = default;
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
