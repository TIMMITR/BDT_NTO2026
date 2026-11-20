using Features.ControlPointPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ControlPointPack.Systems
{
    public class SpawnControlPointSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<GridComponent> _gridStash;
        
        private Request<SpawnControlPointRequest> _spawnControlPointRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            
            _spawnControlPointRequest = World.GetRequest<SpawnControlPointRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _spawnControlPointRequest.Consume())
            {
                var gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                var coord = request.cellCoord;
                var
                
                var obj = Object.Instantiate()
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
