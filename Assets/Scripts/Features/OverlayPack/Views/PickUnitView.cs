using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;
using UnityEngine.UI;

namespace Features.OverlayPack.Views
{
    public sealed class PickUnitView : MonoBehaviour
    {
        [SerializeField] private Button button;

        private Entity _unit;
        private bool _isBound;

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnPickUnit);
        }

        public void Bind(Entity unit, bool canSelect)
        {
            _unit = unit;
            _isBound = true;

            if (button != null)
                button.interactable = canSelect;
        }

        public void Unbind()
        {
            _isBound = false;

            if (button != null)
                button.interactable = false;
        }

        public void OnPickUnit()
        {
            if (!_isBound || World.Default == null)
                return;

            World.Default.GetRequest<PickUnitRequest>().Publish(new PickUnitRequest(_unit));
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnPickUnit);
        }
    }
}
