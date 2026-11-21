using Configs;
using Features.DamagePack;
using Features.GridPack;
using Features.GridPack.Components;
using Features.HealthPack;
using Features.MeshPack;
using Features.SpeedPack;
using Features.SpellPack.Components;
using Features.TransformPack;
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
        
        private Stash<SpellDataComponent> _spellStash;
        private Stash<SpellListComponent> _spellListStash;
        
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
        private Stash<BaseTag> _baseTagStash;
        
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
            _baseTagStash = World.GetStash<BaseTag>();
            
            _spellStash = World.GetStash<SpellDataComponent>();
            _spellListStash = World.GetStash<SpellListComponent>();
            
            _request = World.GetRequest<UnitSpawnRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _request.Consume()) // todo возможно, стоит убрать blocked или удалять клетку из обычных; добавить отдельную систему
            {
                Entity gridEntity = _gridFilter.First();
                ref var gridComp = ref _gridStash.Get(gridEntity);
                
                var unitConfig = request.unitConfig;
                
                var cellCoord = request.cellCoord;
                var spawnPos = gridComp.ToWorldCoord(cellCoord);
                spawnPos.y = unitConfig.PosY;
                
                Entity newUnit = World.CreateEntity();
                var unitObj = Object.Instantiate(unitConfig.Prefab, spawnPos, Quaternion.Euler(0f, unitConfig.SpawnRotationY, 0f));
                var mr = unitObj.GetComponent<MeshRenderer>();
                
                _meshRendererStash.Add(newUnit, new MeshRendererComponent { value = mr });
                _transformStash.Add(newUnit, new TransformComponent { value = unitObj.transform });

                var spells = unitConfig.SpellList;

                if (spells.Count > 0) // todo для сооружений без спелов
                {
                    _spellListStash.Add(newUnit, new SpellListComponent(spells));
                    _spellStash.Add(newUnit, new SpellDataComponent(spells[0]));
                }
                
                ApplyUnitComponents(newUnit, unitConfig);

                if (request.isBase)
                    _baseTagStash.Add(newUnit);
                
                GridOccupancy.Place(ref gridComp, _tileStash, _positionStash, newUnit, cellCoord, request.isBlocked);
            }
        }

        private void ApplyUnitComponents(Entity unit, UnitConfig config)
        {
            var stats = config.Stats;
            
            _unitStash.Add(unit, new UnitComponent(config.Name, config.Class, config.TeamSide, config.PosY));
            
            _healthStash.Add(unit, new HealthComponent(stats.maxHealth));
            _defenceStash.Add(unit, new DefenceComponent{currentDefence = stats.defence});
            _damageStash.Add(unit, new DamageComponent{multiplier = stats.multiplier});
            _speedStash.Add(unit, new SpeedComponent(stats.moveSpeed));
        }

        public void Dispose() { }
    }
}