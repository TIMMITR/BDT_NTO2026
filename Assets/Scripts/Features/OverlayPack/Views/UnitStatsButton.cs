using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class UnitStatsButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private HeroSlotView heroSlot;

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnUnitStats);
        }

        public void OnUnitStats()
        {
            if (heroSlot != null)
                heroSlot.Toggle();
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnUnitStats);
        }
    }
}
