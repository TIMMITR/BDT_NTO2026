using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.CellSelectionPack.Tags;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.InputPack.Requests;
using Features.SpeedPack;
using Features.StatePack.Components;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Global;
using Scellecs.Morpeh;
using SelectUnitRequest = Features.CellSelectionPack.Requests.SelectUnitRequest;

namespace Features.CellSelectionPack.Systems
{
    public class PlayerClickSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<TurnPhaseComponent> _turnPhase; // todo возможно не стоит так выносить
        private Singletone<GridComponent> _grid;
        private Singletone<PlayerSelectionTag> _selection;

        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<PlannedPathComponent> _plannedPathStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<UnitComponent> _unitStash;

        private Stash<BusyTag> _busyTag;
        private Stash<PlayerSelectionTag> _selectionTag;
        
        private Request<SelectUnitRequest> _selectRequest;
        private Request<DeselectUnitRequest> _deselectRequest;
        private Request<MoveUnitRequest> _moveUnitRequest;

        private Event<LmbStartEvent> _lmbEvent;
        
        public void OnAwake()
        {
            _grid = new Singletone<GridComponent>(World);
            _selection = new Singletone<PlayerSelectionTag>(World);
            _turnPhase = new Singletone<TurnPhaseComponent>(World);

            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _plannedPathStash = World.GetStash<PlannedPathComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            
            _busyTag = World.GetStash<BusyTag>();
            _selectionTag = World.GetStash<PlayerSelectionTag>();

            _selectRequest = World.GetRequest<SelectUnitRequest>();
            _deselectRequest = World.GetRequest<DeselectUnitRequest>();
            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
            
            _lmbEvent = World.GetEvent<LmbStartEvent>();

            var ent = World.CreateEntity();
            _selectionTag.Add(ent);
            
            _selection = new Singletone<PlayerSelectionTag>(World);
        }

        public void OnUpdate(float deltaTime)
        {
            var clicked = false;
            
            foreach (var _ in _lmbEvent.publishedChanges)
                clicked = true;

            if (!clicked || _turnPhase.Comp.phase != TurnPhase.Player)
                return;

            var selection = _selection.Entity;

            if (!_chosenCellStash.Has(selection))
                return;

            var cell = _chosenCellStash.Get(selection).cellCoord;
            var isAvailable = IsAvailableUnit(cell, out var unit);

            if (!_selectedUnitStash.Has(selection))
            {
                if (isAvailable)
                    _selectRequest.Publish(new SelectUnitRequest(unit));
                return;
            }

            var selected = _selectedUnitStash.Get(selection).unit;

            if (isAvailable)
            {
                if (unit == selected)
                    _deselectRequest.Publish(new DeselectUnitRequest());
                else
                    _selectRequest.Publish(new SelectUnitRequest(unit));
                return;
            }
            
            // если выбран нейтральный/вражеский юнит

            IssueOrders(selection, selected, cell);
            _deselectRequest.Publish(new DeselectUnitRequest());
        }

        private void IssueOrders(Entity selection, Entity unit, CellCoord target)
        {
            if (!_plannedPathStash.Has(selection))
                return;

            var path = _plannedPathStash.Get(selection).path;

            if (path.Count > 1)
                _moveUnitRequest.Publish(new MoveUnitRequest(unit, new List<CellCoord>(path)));
        }

        // проверяет можно ли выбрать этого юнита
        private bool IsAvailableUnit(CellCoord cell, out Entity unit) => 
            GridData.TryGetUnit(_grid.Comp, _unitStash, cell, out unit, out var side)
            && side == TeamSide.Player
            && !_busyTag.Has(unit)
            && _speedStash.Get(unit).currentSpeed > 0;

        public void Dispose() { }
    }
}