using Features.CellGridPack.Components;
using Features.DamagePack;
using Features.HealthPack;
using Features.HexGridPack.Data;
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
        
        private Stash<HealthComponent> _healthStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<DamageComponent> _damageStash;
        
        private Request<UnitSpawnRequest> _request;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            
            _healthStash = World.GetStash<HealthComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            
            _request = World.GetRequest<UnitSpawnRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume()) // todo возможно, стоит убрать blocked или удалять клетку из обычных; добавить отдельную систему
            {
                Entity gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                var unitConfig = request.unitConfig;
                var stats = unitConfig.Stats;
                var cellCoord = new CellCoord(request.q, request.r);
                var spawnPos = gridComp.ToWorldCoord(cellCoord);
                
                Entity newUnit = World.CreateEntity();
                var unitObj = Object.Instantiate(unitConfig.Prefab, spawnPos, Quaternion.identity);

                _transformStash.Add(newUnit, new TransformComponent { value = unitObj.transform });
                
                _healthStash.Add(newUnit, new HealthComponent(stats.maxHealth));
                _defenceStash.Add(newUnit, new DefenceComponent{currentDefence = stats.defence});
                _damageStash.Add(newUnit, new DamageComponent{baseDamage = stats.damage});
                _unitStash.Add(newUnit, new UnitComponent(unitConfig.Name, unitConfig.Class));
                
                gridComp.takenCells.Add(cellCoord, newUnit);
            }
        }

        public void Dispose() { }
    }
}