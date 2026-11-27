using Configs;
using Features.CellSelectionPack.Requests;
using Features.OverlayPack.Views;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.OverlayPack.Systems
{
    public sealed class UnitPanelSystem : ISystem
    {
        public World World { get; set; }

        private readonly PickUnitsPanelView _view;

        private Filter _unitFilter;
        private Stash<UnitComponent> _unitStash;
        private Request<SelectUnitRequest> _selectUnitRequest;

        private Entity[] _units;
        private bool[] _occupiedSlots;

        public UnitPanelSystem(PickUnitsPanelView view)
        {
            _view = view;
        }

        public void OnAwake()
        {
            _unitFilter = World.Filter.With<UnitComponent>().Build();
            _unitStash = World.GetStash<UnitComponent>();
            _selectUnitRequest = World.GetRequest<SelectUnitRequest>();

            _units = new Entity[_view.Capacity];
            _occupiedSlots = new bool[_view.Capacity];

            _view.UnitClicked += OnUnitClicked;
            _view.ClearBindings();
        }

        public void OnUpdate(float deltaTime)
        {
            RemoveDestroyedUnits();
            RegisterNewUnits();
        }

        private void RemoveDestroyedUnits()
        {
            for (var i = 0; i < _units.Length; i++)
            {
                if (!_occupiedSlots[i])
                    continue;

                if (!World.IsDisposed(_units[i]) && _unitStash.Has(_units[i]))
                    continue;

                _units[i] = default;
                _occupiedSlots[i] = false;
                _view.SetBound(i, false);
            }
        }

        private void RegisterNewUnits()
        {
            foreach (var unit in _unitFilter)
            {
                if (_unitStash.Get(unit).teamSide != TeamSide.Player || IsRegistered(unit))
                    continue;

                var slot = FindFreeSlot();
                if (slot < 0)
                    return;

                _units[slot] = unit;
                _occupiedSlots[slot] = true;
                _view.SetBound(slot, true);
            }
        }

        private bool IsRegistered(Entity unit)
        {
            for (var i = 0; i < _units.Length; i++)
            {
                if (_occupiedSlots[i] && _units[i] == unit)
                    return true;
            }

            return false;
        }

        private int FindFreeSlot()
        {
            for (var i = 0; i < _occupiedSlots.Length; i++)
            {
                if (!_occupiedSlots[i])
                    return i;
            }

            return -1;
        }

        private void OnUnitClicked(int slot)
        {
            if (slot < 0 || slot >= _units.Length || !_occupiedSlots[slot])
                return;

            var unit = _units[slot];
            if (World.IsDisposed(unit) || !_unitStash.Has(unit))
                return;

            _selectUnitRequest.Publish(new SelectUnitRequest(unit));
        }

        public void Dispose()
        {
            if (_view == null)
                return;

            _view.UnitClicked -= OnUnitClicked;
            _view.ClearBindings();
        }
    }
}
