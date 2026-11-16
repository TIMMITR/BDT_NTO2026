using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
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
        private Stash<HoveredCellComponent> _chosenCellStash;
    
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
            _chosenCellStash = World.GetStash<HoveredCellComponent>();
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
            var raycastCoord = raycastComp.hitPoint;

            var gridEntity = _gridFilter.First();
            ref var gridComp = ref _gridStash.Get(gridEntity);
            var transCoord = gridComp.FromWorldCoord(raycastCoord);
            

            if (gridComp.cells.ContainsKey(transCoord))
            {
                _chosenCellStash.Set(gridEntity, new HoveredCellComponent{cellCoord = transCoord});
            }
            else
            {
                _chosenCellStash.Remove(gridEntity);
            }
        }
        
        public void Dispose() {}
    }
}
