using Features.DamagePack;
using Features.FightPack.Requests;
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
        private Stash<DamageComponent> _damageStash;
        
        private Request<AttackOrderRequest> _attackRequest;
        private Request<DamageRequest> _damageRequest;
        
        public void OnAwake()
        {
            _spellStash = World.GetStash<SpellDataComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            
            _attackRequest = World.GetRequest<AttackOrderRequest>();
            _damageRequest = World.GetRequest<DamageRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _attackRequest.Consume())
            {
                ref var spell = ref _spellStash.Get(request.source);
                
                _damageRequest.Publish(new DamageRequest(request.source, request.target, spell.spellData.damage));
                
                Debug.Log($"{request.source} attacking {request.target} with {spell.spellData.damage}");
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
