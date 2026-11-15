using Configs;
using Features.CellGridPack.Components;
using Features.DamagePack;
using Features.HealthPack;
using Features.HexGridPack.Data;
using Features.SpeedPack;
using Features.UnitPack.Components;
using Features.UnitPack.Requests;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public sealed class UnitSpawnSystem : ISystem 
    {
        public World World { get; set; }

        private Filter _gridFilter;
        
        private Stash<CellGridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<CellTileComponent> _cellTileStash;
        
        private Stash<HealthComponent> _healthStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<DamageComponent> _damageStash;
        private Stash<SpeedComponent> _speedStash;
        
        private Request<UnitSpawnRequest> _request;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
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
                
                Entity newUnit = World.CreateEntity();
                var unitObj = Object.Instantiate(unitConfig.Prefab, spawnPos, Quaternion.identity);

                _transformStash.Add(newUnit, new TransformComponent { value = unitObj.transform });
                
                ApplyUnitComponents(newUnit, unitConfig);

                var cellEntity = gridComp.cells[cellCoord];
                ref var cellComp = ref _cellTileStash.Get(cellEntity);

                cellComp.blocked = true;
                
                gridComp.takenCells.Add(cellCoord, newUnit);
            }
        }

        private void ApplyUnitComponents(Entity unit, UnitConfig config)
        {
            var stats = config.Stats;
            
            _unitStash.Add(unit, new UnitComponent(config.Name, config.Class, config.TeamSide));
            
            _healthStash.Add(unit, new HealthComponent(stats.maxHealth));
            _defenceStash.Add(unit, new DefenceComponent{currentDefence = stats.defence});
            _damageStash.Add(unit, new DamageComponent{baseDamage = stats.damage});
            _speedStash.Add(unit, new SpeedComponent { currentSpeed = stats.moveSpeed });
        }

        public void Dispose() { }
    }
}