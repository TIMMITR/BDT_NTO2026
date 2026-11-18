using Configs;
using Features.DamagePack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.HealthPack;
using Features.MeshPack;
using Features.SpeedPack;
using Features.TransformPack;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public sealed class UnitSpawnSystem : ISystem 
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<CellTileComponent> _tileStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<MeshRendererComponent> _meshRendererStash;
        
        private Stash<HealthComponent> _healthStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<DamageComponent> _damageStash;
        private Stash<SpeedComponent> _speedStash;
        
        private Request<UnitSpawnRequest> _request;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _tileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _meshRendererStash = World.GetStash<MeshRendererComponent>();
            
            _healthStash = World.GetStash<HealthComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            
            _request = World.GetRequest<UnitSpawnRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume()) // todo возможно, стоит убрать blocked или удалять клетку из обычных; добавить отдельную систему
            {
                Entity gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                var unitConfig = request.unitConfig;
                
                var cellCoord = new CellCoord(request.q, request.r);
                var spawnPos = gridComp.ToWorldCoord(cellCoord);
                spawnPos.y = unitConfig.PosY;
                
                Entity newUnit = World.CreateEntity();
                var unitObj = Object.Instantiate(unitConfig.Prefab, spawnPos, Quaternion.identity);
                var mr = unitObj.GetComponent<MeshRenderer>();
                
                _meshRendererStash.Add(newUnit, new MeshRendererComponent { value = mr });
                _transformStash.Add(newUnit, new TransformComponent { value = unitObj.transform });
                
                ApplyUnitComponents(newUnit, unitConfig);

                GridOccupancy.Place(ref gridComp, _tileStash, _positionStash, newUnit, cellCoord, request.isBlocked);
            }
        }

        private void ApplyUnitComponents(Entity unit, UnitConfig config)
        {
            var stats = config.Stats;
            
            _unitStash.Add(unit, new UnitComponent(config.Name, config.Class, config.TeamSide));
            
            _healthStash.Add(unit, new HealthComponent(stats.maxHealth));
            _defenceStash.Add(unit, new DefenceComponent{currentDefence = stats.defence});
            _damageStash.Add(unit, new DamageComponent{baseDamage = stats.damage});
            _speedStash.Add(unit, new SpeedComponent(stats.moveSpeed));
        }

        public void Dispose() { }
    }
}