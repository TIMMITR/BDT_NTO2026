using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class PickUnitsPanelView : MonoBehaviour
    {
        [SerializeField] private List<PickUnitView> pickUnitViews;

        public event Action<int> UnitClicked;

        public int Capacity => pickUnitViews.Count;

        private void Awake()
        {
            for (var i = 0; i < pickUnitViews.Count; i++)
            {
                var index = i;
                pickUnitViews[i].pickUnitButton.onClick.AddListener(() => UnitClicked?.Invoke(index));
            }

            ClearBindings();
        }

        public void SetBound(int index, bool bound)
        {
            pickUnitViews[index].pickUnitButton.interactable = bound;
        }

        public void ClearBindings()
        {
            foreach (var view in pickUnitViews)
                view.pickUnitButton.interactable = false;
        }
    }
}