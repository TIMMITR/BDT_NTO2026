using Features.HexGridPack.Components;
using Features.LevelPack.Components;
using Features.LevelPack.Requests;
using Features.RaycastPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PickCellPack
{
    public class ChooseCellSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _raycastFilter;
        private Filter _gridFilter;
        
        private Stash<RaycastComponent> _raycastStash;
        private Stash<HexGridComponent> _gridStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearRequest;
    
        public void OnAwake()
        {
            _raycastFilter = World.Filter.
                With<RaycastComponent>().
                Build();
            _gridFilter = World.Filter.
                With<HexGridComponent>().
                Build();
            
            _raycastStash = World.GetStash<RaycastComponent>();
            _gridStash = World.GetStash<HexGridComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearRequest = World.GetRequest<ClearColorsRequest>();
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

            if (gridComp.cells.TryGetValue(transCoord, out var cellEntity))
            {
                Debug.DrawRay(raycastComp.hitPoint, Vector3.up * 2f, Color.green);
                Debug.DrawRay(gridComp.ToWorldCoord(transCoord), Vector3.up * 2f, Color.red);
                
                ref var chosenCellComp = ref _chosenCellStash.Get(gridEntity);
                
                if(chosenCellComp.cellEntity != cellEntity)
                    _clearRequest.Publish(new ClearColorsRequest());
                
                chosenCellComp.cellEntity = cellEntity;
                
                _colorCellRequest.Publish(new ColorCellRequest {cellCoord = transCoord});
                
                Debug.Log($"Picked cell: {transCoord.q}, {transCoord.r}");
            }
            else
            {
                _clearRequest.Publish(new ClearColorsRequest());
                //Debug.Log("[CHOOSE_CELL_SYSTEM] Cell not found.");
            }
        }
        
        public void Dispose() {}
    }
}
