using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Tags;
using Features.CellTrackPack.Events;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.RaycastPack;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellSelectionPack.Systems
{
    public class ChooseCellSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _raycastFilter;
        
        private Singletone<PlayerSelectionTag> _selection;
        private Singletone<GridComponent> _grid;
        
        private Stash<RaycastComponent> _raycastStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Event<CellChooseChangedEvent> _cellChooseChanged;
        
        private CellCoord _lastCoord;

        private bool _hasChanged;
    
        public void OnAwake()
        {
            _raycastFilter = World.Filter.
                With<RaycastComponent>().
                Build();

            _selection = new Singletone<PlayerSelectionTag>(World);
            _grid = new Singletone<GridComponent>(World);
            
            _raycastStash = World.GetStash<RaycastComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            
            _cellChooseChanged = World.GetEvent<CellChooseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_raycastFilter.IsEmpty())
            {
                Debug.Log("[CHOOSE_CELL_SYSTEM] Can not find one of filter.");
                return;
            }

            var raycastEntity = _raycastFilter.First();
            ref var raycastComp = ref _raycastStash.Get(raycastEntity);
            var hitPoint = raycastComp.hitPoint;
            
            var selected = _selection.Entity;

            if (!raycastComp.hasHit)
            {
                _chosenCellStash.Remove(selected);
                _hasChanged = false;
                return;
            }
            
            var transCoord = _grid.Comp.FromWorldCoord(hitPoint);

            if (_grid.Comp.cells.ContainsKey(transCoord))
                _chosenCellStash.Set(selected, new ChosenCellComponent{cellCoord = transCoord});
            else
                _chosenCellStash.Remove(selected);
            
            if(_lastCoord != transCoord || !_hasChanged)
                _cellChooseChanged.NextFrame(new CellChooseChangedEvent(transCoord));
            
            _lastCoord = transCoord;
            _hasChanged = true;
        }
        
        public void Dispose() {}
    }
}
