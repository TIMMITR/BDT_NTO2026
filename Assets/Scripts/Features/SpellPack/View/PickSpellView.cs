using System;
using Configs;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.SpellPack.View
{
    public sealed class PickSpellView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button pickButton;
        [SerializeField] private Image icon;
        [SerializeField] private float hoverDelay = 0.5f;

        private SpellConfigSo _spell;
        private Tween _delay;
        private bool _isHovered;

        public event Action Clicked;
        public event Action<SpellConfigSo> TooltipRequested;
        public event Action TooltipCancelled;

        private void Awake()
        {
            pickButton.onClick.AddListener(OnClick);
        }

        public void Bind(SpellConfigSo spell, bool selected)
        {
            CancelHover();

            _spell = spell;
            gameObject.SetActive(spell != null);
            pickButton.interactable = spell != null;

            if (icon != null)
            {
                icon.sprite = spell != null ? spell.Icon : null;
                icon.enabled = icon.sprite != null;
            }

            var colors = pickButton.colors;
            colors.normalColor = selected ? new Color(1f, 0.85f, 0.45f) : Color.white;
            pickButton.colors = colors;
        }

        public void Clear()
        {
            Bind(null, false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_spell == null)
                return;

            CancelHover();
            _isHovered = true;
            _delay = Tween.Delay(this, hoverDelay, view => view.RequestTooltip(), useUnscaledTime: true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelHover();
        }

        private void RequestTooltip()
        {
            if (_isHovered && _spell != null)
                TooltipRequested?.Invoke(_spell);
        }

        private void OnClick()
        {
            if (_spell != null)
                Clicked?.Invoke();
        }

        private void CancelHover()
        {
            _isHovered = false;
            if (_delay.isAlive)
                _delay.Stop();
            _delay = default;
            TooltipCancelled?.Invoke();
        }

        private void OnDisable()
        {
            CancelHover();
        }

        private void OnDestroy()
        {
            if (_delay.isAlive)
                _delay.Stop();
            pickButton.onClick.RemoveListener(OnClick);
        }
    }
}
