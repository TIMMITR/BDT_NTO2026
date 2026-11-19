using Features.DamagePack;
using Features.FightPack.Requests;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.KnockbackPack.Requests;
using Features.SpellPack.Components;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.FightPack.Systems
{
    public class AttackSystem : ISystem
    {
        public World World { get; set; }

        private Stash<SpellDataComponent> _spellStash;
        private Stash<CellPositionComponent> _positionStash;

        private Request<KnockbackRequest> _knockbackRequest;
        private Request<AttackOrderRequest> _attackRequest;
        private Request<DamageRequest> _damageRequest;
        
        public void OnAwake()
        {
            _spellStash = World.GetStash<SpellDataComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _knockbackRequest = World.GetRequest<KnockbackRequest>();
            _attackRequest = World.GetRequest<AttackOrderRequest>();
            _damageRequest = World.GetRequest<DamageRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _attackRequest.Consume())
            {
                ref var spell = ref _spellStash.Get(request.source);
                var spellData = spell.spellData;
                
                _damageRequest.Publish(new DamageRequest(request.source, request.target, spellData.damage));

                if (spell.spellData.punchDistance > 0)
                {
                    var target = _positionStash.Get(request.target).coord;
                    var dir = request.origin.DirectionTo(target);
                    
                    _knockbackRequest.Publish(new KnockbackRequest(request.source, request.target, dir, 
                        spellData.punchDistance, spellData.punchDamage, spellData.punchReturnDistance));
                }
                
                Debug.Log($"{request.source} attacking {request.target} with {spell.spellData.damage}");
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
