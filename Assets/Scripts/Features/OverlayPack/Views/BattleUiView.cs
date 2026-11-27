using Features.SpellPack.View;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class BattleUiView : MonoBehaviour
    {
        [SerializeField] private GameObject canvasUI;
        [SerializeField] private PickUnitsPanelView unitsPanel;
        [SerializeField] private SpellPanelView spellPanel;

        public PickUnitsPanelView UnitsPanel => unitsPanel;
        public SpellPanelView SpellPanel => spellPanel;

        public void SetActive(bool active)
        {
            canvasUI.SetActive(active);
        }
    }

    public struct BattleUiRequest : IRequestData
    {
        public bool active;
    }
}