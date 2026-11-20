using Configs;
using Features.ControlPointPack.Components;
using Features.ControlPointPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.TransformPack;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ControlPointPack.Systems
{
    public class SpawnControlPointSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<ControlPointComponent> _controlPointStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<CellTileComponent> _cellTileStash;
        
        private Request<SpawnControlPointRequest> _spawnControlPointRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _controlPointStash = World.GetStash<ControlPointComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
            _spawnControlPointRequest = World.GetRequest<SpawnControlPointRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _spawnControlPointRequest.Consume())
            {
                var gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                var coord = gridComp.ToWorldCoord(request.cellCoord);
                var config = request.config;
                
                var obj = Object.Instantiate(config.prefab, coord, Quaternion.identity);
                
                var entity = World.CreateEntity();
                _controlPointStash.Add(entity, new ControlPointComponent(request.cellCoord, TeamSide.None));
                _transformStash.Add(entity, new TransformComponent(obj.transform));
                
                _cellTileStash.Get(gridComp.cells[request.cellCoord]).blocked = true;
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
