using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Events;
using Features.CellSelectionPack.Requests;
using Features.FightPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.InputPack.Requests;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.StatePack.Components;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.UnitPack.Systems
{
    public sealed class SelectUnitSystem : ISystem // todo этот класс надо полностью переписать: разделить на ивенты и доп системы
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _turnPhaseFilter;
        
        private Stash<TurnPhaseComponent> _turnPhaseStash;

        private Stash<GridComponent> _gridStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;
        private Stash<DelayAttackComponent> _delayAttackStash;
        private Stash<SpellDataComponent> _spellDataStash;

        private Request<LmbStartRequest> _lmbRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;
        private Request<ColorCellRequest> _colorCellRequest;

        private Event<CellChooseChangedEvent> _cellChoosedChangedEvent;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _turnPhaseFilter = World.Filter.With<TurnPhaseComponent>().Build();
            
            _turnPhaseStash = World.GetStash<TurnPhaseComponent>();

            _gridStash = World.GetStash<GridComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();
            _delayAttackStash = World.GetStash<DelayAttackComponent>();
            _spellDataStash = World.GetStash<SpellDataComponent>();

            _lmbRequest = World.GetRequest<LmbStartRequest>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
            _colorCellRequest = World.GetRequest<ColorCellRequest>();

            _cellChoosedChangedEvent = World.GetEvent<CellChooseChangedEvent>();
        }

        public void OnUpdate(float deltaTime) // todo переписать
        {
            ref var turnPhaseComp = ref _turnPhaseStash.Get(_turnPhaseFilter.First());
            
            if(turnPhaseComp.phase != TurnPhase.Player)
                return;

            var lmbClicked = false;
            foreach (var _ in _lmbRequest.Consume()) lmbClicked = true;
            
            var gridEntity = _gridFilter.First();

            if (!lmbClicked || !_chosenCellStash.Has(gridEntity)) return;
            
            ref var gridComp = ref _gridStash.Get(gridEntity);
            var clicked = _chosenCellStash.Get(gridEntity).cellCoord;

            bool hadPickedCell = _selectedUnitStash.Has(gridEntity);

            if (hadPickedCell)
            {
                var selected = _selectedUnitStash.Get(gridEntity);

                var teamSide = TryGetUnit(gridComp, clicked, out var unit);

                // если второй клеткой выбран союзный юнит, то перезаписываем данные для нового юнита
                if (teamSide == TeamSide.Player)
                {
                    if (unit == selected.unit)
                    {
                        _selectedUnitStash.Remove(gridEntity);
                        _cellChoosedChangedEvent.NextFrame(new CellChooseChangedEvent(clicked));
                        return;
                    }
                    
                    if (TryGetPlayerSpeed(unit, out var moveSpeed))
                    {
                        _selectedUnitStash.Set(gridEntity, new SelectedUnitComponent(unit, teamSide, moveSpeed, selected.path));
                        return;
                    }
                }
                else if (_spellDataStash.Has(selected.unit))
                {
                    _delayAttackStash.Set(selected.unit, new DelayAttackComponent(unit));
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
                var teamSide = TryGetUnit(gridComp, clicked, out var unit);
                
                if (teamSide != TeamSide.Player || !TryGetPlayerSpeed(unit, out var moveSpeed))
                    return;

                _selectedUnitStash.Set(gridEntity, new SelectedUnitComponent(unit, teamSide, moveSpeed));
                
                var pos = _positionStash.Get(unit);
                
                _colorCellRequest.Publish(new ColorCellRequest(pos.coord, ColorType.Clicked));
            }
        }

        private TeamSide TryGetUnit(GridComponent gridComp, CellCoord cellCoord, out Entity unit)
        {
            if (!gridComp.takenCells.TryGetValue(cellCoord, out unit))
                return TeamSide.None;
            if (_unitAnimationStash.Has(unit))
                return TeamSide.None;

            ref var unitComp = ref _unitStash.Get(unit);
            
            return unitComp.teamSide;
        }

        private bool TryGetPlayerSpeed(Entity unit, out int moveSpeed)
        {
            moveSpeed = _speedStash.Get(unit).currentSpeed;
            if (moveSpeed <= 0)
                return false;
            
            return true;
        }

        public void Dispose() {}
    }
}