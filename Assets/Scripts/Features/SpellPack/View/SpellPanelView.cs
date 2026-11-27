using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Features.SpellPack.View
{
    public sealed class SpellPanelView : MonoBehaviour
    {
        [SerializeField] private List<Button> spellButtons;

        public event Action<int> spellClicked;

        private void Awake()
        {
            for (var i = 0; i < spellButtons.Count; i++)
            {
                var index = i;
                spellButtons[i].onClick.AddListener(() => spellClicked?.Invoke(index));
            }
        }
    }
}