using UnityEngine;
using Features.GridPack.Components;
using Features.HealthPack;
using Features.TransformPack;
using Scellecs.Morpeh;

namespace Features.DeathPack.Systems
{
    public class DeathSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<CellTileComponent> _cellTileStash;
        
        private Event<HealthChangedEvent> _healthChangedEvent;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
            _healthChangedEvent = World.GetEvent<HealthChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _healthChangedEvent.publishedChanges)
            {
                var unit = evt.entity;
                
                if(evt.newAmount > 0 || World.IsDisposed(unit))
                    continue;
                
                var gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);


                if (_positionStash.Has(unit))
                {
                    var coord = _positionStash.Get(unit).coord;
                    gridComp.takenCells.Remove(coord);
                }
                
                Object.Destroy(_transformStash.Get(unit).value.gameObject);
                
                World.RemoveEntity(unit);
            }
        }
    
        public void Dispose() {}
    }
}
