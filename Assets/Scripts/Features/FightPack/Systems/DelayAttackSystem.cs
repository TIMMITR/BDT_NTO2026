using System.Collections.Generic;
using Configs;
using Features.CellSelectionPack.Components;
using Features.FightPack.Components;
using Features.FightPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
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

        private List<Entity> _targets = new();
        
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
                ref var attackDesire = ref _attackDesireStash.Get(unit);
                
                var from = _positionStash.Get(unit).coord;
                var to = attackDesire.target;
                var spell = attackDesire.spellConfig;

                if (!SpellArea.TryAim(_grid.Comp, _cellTileStash, spell, from, to,
                        out var dir))
                    continue;
                
                _targets.Clear();

                var team = _unitStash.Get(unit).teamSide;
                
                SpellArea.CollectTargets(_grid.Comp, _unitStash, _cellTileStash, spell, from, to, dir, team, _targets);

                var startPoint = SpellArea.GetStartPoint(spell, from, to);
                List<AttackOrderRequest> attackOrders = new(_targets.Count);
                
                foreach (var target in _targets)
                    attackOrders.Add(new AttackOrderRequest(unit, target, startPoint, spell));
                
                _projectileCastRequest.Publish(new ProjectileCastRequest(from, to, spell, attackOrders));
            }
        }
        
        public void Dispose() { }
    }
}
