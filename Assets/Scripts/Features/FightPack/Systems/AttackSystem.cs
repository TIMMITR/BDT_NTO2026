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

        private Stash<CellPositionComponent> _positionStash;

        private Request<KnockbackRequest> _knockbackRequest;
        private Request<AttackOrderRequest> _attackRequest;
        private Request<DamageRequest> _damageRequest;
        
        public void OnAwake()
        {
            _positionStash = World.GetStash<CellPositionComponent>();
            
            _knockbackRequest = World.GetRequest<KnockbackRequest>();
            _attackRequest = World.GetRequest<AttackOrderRequest>();
            _damageRequest = World.GetRequest<DamageRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _attackRequest.Consume())
            {
                var spell = request.spell;
                var source = request.source;
                var target = request.target;
                
                _damageRequest.Publish(new DamageRequest(source, target, spell.damage));

                if (spell.punchDistance > 0 && _positionStash.Has(target))
                {
                    var to = _positionStash.Get(target).coord;
                    var dir = request.origin.DirectionTo(to);
                    
                    _knockbackRequest.Publish(new KnockbackRequest(source, target, dir, 
                        spell.punchDistance, spell.punchDamage, spell.punchReturnDistance));
                }
                
                Debug.Log($"{source} attacking {target} with {spell.damage}");
            }
        }
    
        public void Dispose() {}
    }
}
