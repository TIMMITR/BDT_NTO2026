using System.Collections.Generic;
using Features.SpellPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.SpellPack.View
{
    public sealed class SpellPanelView : MonoBehaviour
    {
        [SerializeField] private List<Button> spellButtons;

        private void Awake()
        {
            for (var i = 0; i < spellButtons.Count; i++)
            {
                var index = i;
                spellButtons[i].onClick.AddListener(() =>
                    World.Default.GetRequest<SpellChangeRequest>().Publish(new SpellChangeRequest(index))
                );
            }
        }
    }
}