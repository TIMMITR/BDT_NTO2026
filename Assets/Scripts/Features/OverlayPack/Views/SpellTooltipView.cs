using Configs;
using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class SpellTooltipView : MonoBehaviour
    {
        [SerializeField] private Text description;

        public void SetData(SpellConfigSo spell, int damageMultiplier)
        {
            if (description == null || spell == null)
                return;

            description.text = $"<b>{spell.DisplayName}</b>\n" +
                               $"Мана: {spell.manaCost}\n" +
                               $"Урон: {spell.damage * damageMultiplier}\n" +
                               $"Дальность: {spell.range}\n" +
                               $"Радиус: {spell.areaRadius}";

            if (spell.punchDistance > 0)
                description.text += $"\nОтбрасывание: {spell.punchDistance}";
            if (spell.ignoreArmor)
                description.text += "\nИгнорирует защиту";
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
