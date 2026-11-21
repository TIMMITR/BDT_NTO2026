using Configs;
using Features.SpellPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class SpellButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private SpellTooltipView tooltip;

        private Entity _unit;
        private SpellConfigSo _spell;
        private int _spellIndex;
        private Sprite _defaultIcon;
        private bool _isBound;

        private void Awake()
        {
            if (icon != null)
                _defaultIcon = icon.sprite;

            if (button != null)
                button.onClick.AddListener(OnSpellClick);
        }

        public void Bind(Entity unit, SpellConfigSo spell, int spellIndex, int damageMultiplier, bool canSelect)
        {
            _unit = unit;
            _spell = spell;
            _spellIndex = spellIndex;
            _isBound = true;

            if (button != null)
                button.interactable = canSelect;
            if (icon != null)
                icon.sprite = spell.icon != null ? spell.icon : _defaultIcon;
            if (tooltip != null)
                tooltip.SetData(spell, damageMultiplier);
        }

        public void Unbind()
        {
            _isBound = false;
            _spell = null;

            if (button != null)
                button.interactable = false;
            if (icon != null)
                icon.sprite = _defaultIcon;
            if (tooltip != null)
                tooltip.Hide();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_isBound && tooltip != null)
                tooltip.Show();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tooltip != null)
                tooltip.Hide();
        }

        public void OnSpellClick()
        {
            if (!_isBound || World.Default == null)
                return;

            World.Default.GetRequest<SpellChangeRequest>().Publish(
                new SpellChangeRequest(_unit, _spellIndex));
        }

        private void OnDisable()
        {
            if (tooltip != null)
                tooltip.Hide();
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnSpellClick);
        }
    }
}
