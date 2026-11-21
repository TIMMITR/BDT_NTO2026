using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Events;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.RaycastPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellSelectionPack.Systems
{
    public class ChooseCellSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _raycastFilter;
        private Filter _gridFilter;
        
        private Stash<RaycastComponent> _raycastStash;
        private Stash<GridComponent> _gridStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Event<CellChooseChangedEvent> _cellChooseChanged;
        
        private CellCoord _lastCoord;

        private bool _hasChanged;
    
        public void OnAwake()
        {
            _raycastFilter = World.Filter.
                With<RaycastComponent>().
                Build();
            _gridFilter = World.Filter.
                With<GridComponent>().
                Build();
            
            _raycastStash = World.GetStash<RaycastComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            
            _cellChooseChanged = World.GetEvent<CellChooseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_raycastFilter.IsEmpty() || _gridFilter.IsEmpty())
            {
                Debug.Log("[CHOOSE_CELL_SYSTEM] Can not find one of filter.");
                return;
            }

            var raycastEntity = _raycastFilter.First();
            ref var raycastComp = ref _raycastStash.Get(raycastEntity);
            var hitPoint = raycastComp.hitPoint;

            var gridEntity = _gridFilter.First();
            ref var gridComp = ref _gridStash.Get(gridEntity);

            if (!raycastComp.hasHit)
            {
                _chosenCellStash.Remove(gridEntity);
                _hasChanged = false;
                return;
            }
            
            var transCoord = gridComp.FromWorldCoord(hitPoint);

            if (gridComp.cells.ContainsKey(transCoord))
                _chosenCellStash.Set(gridEntity, new ChosenCellComponent{cellCoord = transCoord});
            else
                _chosenCellStash.Remove(gridEntity);
            
            if(_lastCoord != transCoord || !_hasChanged)
                _cellChooseChanged.NextFrame(new CellChooseChangedEvent(transCoord));
            
            _lastCoord = transCoord;
            _hasChanged = true;
        }
        
        public void Dispose() {}
    }
}
