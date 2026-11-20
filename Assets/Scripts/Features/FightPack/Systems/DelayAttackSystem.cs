using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.FightPack.Components;
using Features.FightPack.Requests;
using Features.GridPack.Components;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.FightPack.Systems
{
    public class DelayAttackSystem : ISystem
    {
        public World World { get; set; }

        private Filter _delayAttackUnitFilter;
        private Filter _gridFilter;

        private Stash<DelayAttackComponent> _delayStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<GridComponent> _gridStash;
        private Stash<AttackDataComponent> _attackDataStash;
        private Stash<CellPositionComponent> _positionStash;
        
        private Request<ProjectileCastRequest> _projectileCastRequest;

        public void OnAwake()
        {
            _delayAttackUnitFilter = World.Filter.
                With<DelayAttackComponent>().
                Without<UnitAnimationComponent>().
                Build();

            _gridFilter = World.Filter.With<GridComponent>().Build();

            _delayStash = World.GetStash<DelayAttackComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _attackDataStash = World.GetStash<AttackDataComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _projectileCastRequest = World.GetRequest<ProjectileCastRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _delayAttackUnitFilter)
            {
                var gridEntity = _gridFilter.First();

                ref var gridComp = ref _gridStash.Get(gridEntity);
                ref var attackData = ref _attackDataStash.Get(gridEntity);

                attackData.from = _positionStash.Get(unit).coord;

                if (!SpellArea.TryHitFrom(gridComp, _cellTileStash, //todo возможно стоит попробовать избежать пересчитывания
                        attackData.spellConfig,
                        attackData.from, attackData.to, 
                        out attackData.dir))
                {
                    _delayStash.Remove(unit);
                    continue;
                }
                
                var targets = SpellArea.CollectTargets(gridComp, _cellTileStash, _unitStash, attackData, TeamSide.Player);
                var origin = attackData.spellConfig.rangeType == SpellRangeType.Area 
                    ? attackData.to 
                    : attackData.from;

                List<AttackOrderRequest> orders = new();

                foreach (var target in targets)
                    orders.Add(new AttackOrderRequest(unit, target, origin));
                
                _projectileCastRequest.Publish(new ProjectileCastRequest(attackData.from, attackData.to, attackData.spellConfig, orders));
                
                _delayStash.Remove(unit);
            }
        }
        
        public void Dispose() { }
    }
}
