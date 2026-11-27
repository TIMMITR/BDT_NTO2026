using Features.CameraPack.Components;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Tags;
using Features.GridPack;
using Features.GridPack.Components;
using Features.HealthPack;
using Features.InputPack.Requests;
using Features.OverlayPack.Views;
using Features.SpeedPack;
using Features.TransformPack;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;
using UnityEngine.EventSystems;

namespace Features.OverlayPack.Systems
{
    public sealed class UnitHoverPanelSystem : ISystem
    {
        public World World { get; set; }

        private readonly UnitHoverPanelView _view;

        private Singletone<PlayerSelectionTag> _selection;
        private Singletone<GridComponent> _grid;
        private Filter _cameraFilter;

        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<CameraComponent> _cameraStash;

        private Event<LmbStartEvent> _lmbStartEvent;

        private Entity _unit;
        private bool _isVisible;

        public UnitHoverPanelSystem(UnitHoverPanelView view)
        {
            _view = view;
        }

        public void OnAwake()
        {
            _selection = new Singletone<PlayerSelectionTag>(World);
            _grid = new Singletone<GridComponent>(World);
            _cameraFilter = World.Filter.With<CameraComponent>().With<CameraTag>().Build();

            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _cameraStash = World.GetStash<CameraComponent>();

            _lmbStartEvent = World.GetEvent<LmbStartEvent>();
            _view.Hide();
        }

        public void OnUpdate(float deltaTime)
        {
            var hasHoveredUnit = TryGetHoveredUnit(out var hoveredUnit);
            var clicked = false;

            foreach (var _ in _lmbStartEvent.publishedChanges)
                clicked = true;

            if (clicked)
            {
                _isVisible = hasHoveredUnit;
                if (_isVisible)
                    _unit = hoveredUnit;
            }

            if (!_isVisible)
                return;

            if (!hasHoveredUnit || hoveredUnit != _unit || !CanShow(_unit) || IsPointerOverUI())
            {
                Hide();
                return;
            }

            var unitData = _unitStash.Get(_unit);
            var health = _healthStash.Get(_unit);
            var defence = _defenceStash.Get(_unit);
            var speed = _speedStash.Get(_unit);

            _view.Show(
                unitData.characterName,
                health.currentHealth,
                health.maxHealth,
                defence.currentDefence,
                speed.currentSpeed,
                speed.maxSpeed);

            if (_cameraFilter.IsEmpty()) return;
            var camera = _cameraStash.Get(_cameraFilter.First()).value;
            _view.SetWorldPosition(camera, _transformStash.Get(_unit).value.position);
        }

        private bool TryGetHoveredUnit(out Entity unit)
        {
            unit = default;
            var selection = _selection.Entity;

            return _chosenCellStash.Has(selection)
                   && GridData.TryGetUnit(
                       _grid.Comp,
                       _unitStash,
                       _chosenCellStash.Get(selection).cellCoord,
                       out unit);
        }

        private bool CanShow(Entity unit)
        {
            return !World.IsDisposed(unit)
                   && _unitStash.Has(unit)
                   && _healthStash.Has(unit)
                   && _defenceStash.Has(unit)
                   && _speedStash.Has(unit)
                   && _transformStash.Has(unit);
        }

        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void Hide()
        {
            _isVisible = false;
            _unit = default;
            _view.Hide();
        }

        public void Dispose()
        {
            if (_view != null)
                _view.Hide();
        }
    }
}
