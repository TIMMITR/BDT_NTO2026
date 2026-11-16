using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.RaycastPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellPack.Systems
{
    public class ChooseCellSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _raycastFilter;
        private Filter _gridFilter;
        
        private Stash<RaycastComponent> _raycastStash;
        private Stash<CellGridComponent> _gridStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
    
        public void OnAwake()
        {
            _raycastFilter = World.Filter.
                With<RaycastComponent>().
                Build();
            _gridFilter = World.Filter.
                With<CellGridComponent>().
                Build();
            
            _raycastStash = World.GetStash<RaycastComponent>();
            _gridStash = World.GetStash<CellGridComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
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
                _chosenCellStash.Set(gridEntity, new ChosenCellComponent{cellCoord = transCoord});
            }
            else
            {
                _chosenCellStash.Remove(gridEntity);
            }
        }
        
        public void Dispose() {}
    }
}
