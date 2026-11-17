using Configs;
using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.InputPack.Requests;
using Features.SpeedPack;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public sealed class SelectUnitSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;

        private Stash<GridComponent> _gridStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;

        private Request<LmbStartRequest> _lmbRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();

            _gridStash = World.GetStash<GridComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();

            _lmbRequest = World.GetRequest<LmbStartRequest>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gridFilter.IsEmpty()) return;

            var lmbClicked = false;
            foreach (var request in _lmbRequest.Consume())
                lmbClicked = true;

            var gridEntity = _gridFilter.First();

            if (!lmbClicked || !_chosenCellStash.Has(gridEntity)) return;

            ref var gridComp = ref _gridStash.Get(gridEntity);
            var clicked = _chosenCellStash.Get(gridEntity).cellCoord;

            if (_selectedUnitStash.Has(gridEntity))
            {
                var selected = _selectedUnitStash.Get(gridEntity);

                // если второй клеткой выбран союзный юнит, то перезаписываем данные для нового юнита
                if (TryGetPlayerUnit(gridComp, clicked, out var newUnit, out var moveSpeed))
                {
                    _selectedUnitStash.Set(gridEntity, new SelectedUnitComponent(newUnit, moveSpeed, selected.path));
                    return;
                }

                // двигаем
                if (selected.path != null && selected.path.Count > 1)
                {
                    _moveUnitRequest.Publish(new MoveUnitRequest(selected.unit, selected.path));
                }

                _selectedUnitStash.Remove(gridEntity);
            }
            else
            {
                // первый раз выбрали клетку с союзным юнитом
                if (!TryGetPlayerUnit(gridComp, clicked, out var unit, out var moveSpeed)) return;

                _selectedUnitStash.Set(gridEntity, new SelectedUnitComponent(unit, moveSpeed));
            }
        }

        private bool TryGetPlayerUnit(in GridComponent gridComp, CellCoord cellCoord, out Entity unit, out int moveSpeed)
        {
            unit = default;
            moveSpeed = 0;

            if (!gridComp.takenCells.TryGetValue(cellCoord, out unit)) return false;
            if (_unitAnimationStash.Has(unit)) return false;

            ref var unitComp = ref _unitStash.Get(unit);
            if (unitComp.teamSide != TeamSide.Player) return false;

            moveSpeed = _speedStash.Get(unit).currentSpeed;
            return true;
        }

        public void Dispose() {}
    }
}