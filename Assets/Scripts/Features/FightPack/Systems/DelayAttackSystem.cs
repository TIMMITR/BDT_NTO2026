using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.FightPack.Components;
using Features.FightPack.Requests;
using Features.GridPack.Components;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;
using Unity.VisualScripting;

namespace Features.FightPack.Systems
{
    public class DelayAttackSystem : ISystem
    {
        public World World { get; set; }

        private Filter _attackReadyFilter;
        private Singletone<GridComponent> _grid;

        private Stash<AttackDesireComponent> _attackDesireStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<GridComponent> _gridStash;
        private Stash<CellTileComponent> _cellTileStash;
        private Stash<CellPositionComponent> _positionStash;
        
        private Request<ProjectileCastRequest> _projectileCastRequest;

        public void OnAwake()
        {
            _attackReadyFilter = World.Filter.
                With<AttackDesireComponent>().
                With<UnitComponent>().
                With<CellPositionComponent>().
                Without<UnitComponent>().
                Build();
            
            _grid = new Singletone<GridComponent>(World);

            _attackDesireStash = World.GetStash<AttackDesireComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _projectileCastRequest = World.GetRequest<ProjectileCastRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var unit in _attackReadyFilter)
            {
                
                ref var attackData = ref _attackDataStash.Get(gridEntity);

                attackData.from = _positionStash.Get(unit).coord;

                if (!SpellArea.TryHitFrom(gridComp, _cellTileStash, // todo возможно стоит попробовать избежать пересчитывания
                        attackData.spellConfig,
                        attackData.from, attackData.to, 
                        out attackData.dir))
                {
                    _delayStash.Remove(unit);
                    continue;
                }
                
                var casterSide = _unitStash.Get(unit).teamSide;
                var targets = SpellArea.CollectTargets(gridComp, _cellTileStash, _unitStash, attackData, casterSide);
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
