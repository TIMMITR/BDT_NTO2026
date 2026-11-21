using Features.ManaPack.Events;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.ManaPack
{
    public class ManaView : MonoBehaviour
    {
        public Text manaText;
        private Event<ManaChangedEvent> _manaEvent;

        public void Awake()
        {
            _manaEvent = World.Default.GetEvent<ManaChangedEvent>();
        }

        public void Update()
        {
            foreach (var evt in _manaEvent.publishedChanges)
            {
                manaText.text = $"Mana: {evt.newAmount}";
            }
        }
    }
}