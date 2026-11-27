using System;
using System.Collections.Generic;
using UnityEngine;

namespace Features.OverlayPack.Views
{
    public class PickUnitsPanelView : MonoBehaviour
    {
        [SerializeField] private List<PickUnitView> pickUnitViews;

        public event Action<int> unitClicked;

        public int Capacity => pickUnitViews.Count;

        private void Awake()
        {
            for (var i = 0; i < pickUnitViews.Count; i++)
            {
                var index = i;
                pickUnitViews[i].pickUnitButton.onClick.AddListener(() => unitClicked?.Invoke(index));
            }

            ClearBindings();
        }

        public void SetBound(int index, bool bound)
        {
            pickUnitViews[index].pickUnitButton.interactable = bound;
            if (!bound)
                pickUnitViews[index].ClearStats();
        }

        public void UpdateStats(
            Sprite sprite,
            int index,
            int currentHealth,
            int maxHealth,
            int defence,
            int currentSpeed,
            int maxSpeed)
        {
            pickUnitViews[index].UpdateStats(sprite, currentHealth, maxHealth, defence, currentSpeed, maxSpeed);
        }

        public void ClearBindings()
        {
            foreach (var view in pickUnitViews)
            {
                view.pickUnitButton.interactable = false;
                view.ClearStats();
            }
        }
    }
}