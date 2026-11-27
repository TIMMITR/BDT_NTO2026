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
        [SerializeField] private UnitHoverPanelView unitHoverPanel;

        public PickUnitsPanelView UnitsPanel => unitsPanel;
        public SpellPanelView SpellPanel => spellPanel;
        public UnitHoverPanelView UnitHoverPanel => unitHoverPanel;

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