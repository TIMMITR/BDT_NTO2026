using System;
using Features.ManaPack.Events;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.ManaPack
{
    public class ManaView : MonoBehaviour
    {
        [SerializeField] private Text manaText;

        private IDisposable _subscription;

        private void Awake()
        {
            _subscription = World.Default
                .GetEvent<ManaChangedEvent>()
                .Subscribe(changes =>
                {
                    for (var i = 0; i < changes.length; i++)
                    {
                        var evt = changes.data[i];
                        manaText.text = evt.newAmount.ToString();
                    }
                });
        }

        private void OnDestroy()
        {
            _subscription?.Dispose();
        }
    }
}