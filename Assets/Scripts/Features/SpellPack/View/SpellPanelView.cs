using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;

namespace Features.SpellPack.View
{
    public sealed class SpellPanelView : MonoBehaviour
    {
        [SerializeField] private List<PickSpellView> spellViews;
        [SerializeField] private SpellTooltipView tooltip;

        public event Action<int> spellClicked;

        private void Awake()
        {
            for (var i = 0; i < spellViews.Count; i++)
            {
                var index = i;
                spellViews[i].Clicked += () => spellClicked?.Invoke(index);
                spellViews[i].TooltipRequested += tooltip.Show;
                spellViews[i].TooltipCancelled += tooltip.Hide;
            }

            Clear();
        }

        public void Bind(IReadOnlyList<SpellConfigSo> spells, int selectedIndex)
        {
            for (var i = 0; i < spellViews.Count; i++)
            {
                var spell = i < spells.Count ? spells[i] : null;
                spellViews[i].Bind(spell, i == selectedIndex);
            }
        }

        public void Clear()
        {
            tooltip.HideImmediately();
            foreach (var view in spellViews)
                view.Clear();
        }
    }
}
