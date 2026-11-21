using Configs;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public sealed class SpellSlotView : MonoBehaviour
    {
        [SerializeField] private SpellButtonView spellButton;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Bind(Entity unit, SpellConfigSo spell, int spellIndex, int damageMultiplier, bool canSelect)
        {
            gameObject.SetActive(true);
            spellButton.Bind(unit, spell, spellIndex, damageMultiplier, canSelect);
        }

        public void Hide()
        {
            spellButton.Unbind();
            gameObject.SetActive(false);
        }
    }
}
