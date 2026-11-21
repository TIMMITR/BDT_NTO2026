using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class HeroSlotView : MonoBehaviour
    {
        [SerializeField] private PickUnitView unitButton;
        [SerializeField] private RectTransform statsPanel;
        [SerializeField] private RectTransform openButton;
        [SerializeField] private Text healthText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text defenceText;
        [SerializeField] private Vector2 openButtonOpenedPosition = new(325f, 50f);
        [SerializeField] private Vector2 openButtonClosedPosition = new(100f, 50f);

        private Entity _unit;
        private bool _isBound;
        private bool _isOpen;
        private bool _wasClosedManually;

        public Entity Unit => _unit;
        public bool IsBound => _isBound;

        private void Awake()
        {
            SetVisible(false);
        }

        public void Bind(Entity unit, bool canSelect)
        {
            if (!_isBound || _unit != unit)
            {
                _unit = unit;
                _wasClosedManually = false;
                SetVisible(false);
            }

            _isBound = true;
            if (unitButton != null)
                unitButton.Bind(unit, canSelect);
        }

        public void Unbind()
        {
            _isBound = false;
            _wasClosedManually = false;
            SetVisible(false);

            if (unitButton != null)
                unitButton.Unbind();
        }

        public void SetStats(int health, int maxHealth, int speed, int maxSpeed, int defence)
        {
            if (healthText != null)
                healthText.text = $"{health}/{maxHealth}";
            if (speedText != null)
                speedText.text = $"{speed}/{maxSpeed}";
            if (defenceText != null)
                defenceText.text = defence.ToString();
        }

        public void Toggle()
        {
            if (!_isBound)
                return;

            _wasClosedManually = _isOpen;
            SetVisible(!_isOpen);
        }

        public void OpenAutomatically()
        {
            if (_isBound && !_wasClosedManually)
                SetVisible(true);
        }

        public void ResetForNewTurn()
        {
            _wasClosedManually = false;
        }

        private void SetVisible(bool visible)
        {
            _isOpen = visible;

            if (statsPanel != null)
                statsPanel.gameObject.SetActive(visible);
            if (openButton != null)
                openButton.anchoredPosition = visible ? openButtonOpenedPosition : openButtonClosedPosition;
        }
    }
}
