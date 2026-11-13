using System;
using Features.HealthPack;
using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public sealed class DamageSystem : ISystem
    {
        public World World { get; set; }

        private Stash<DamageComponent> _damageStash;
        private Stash<DefenceComponent> _defenceStash;
        
        private Request<DamageRequest> _damageRequest;

        public void OnAwake()
        {
            _damageStash = World.GetStash<DamageComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _damageRequest = World.GetRequest<DamageRequest>();
            
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _damageRequest.Consume())
            {
                if (!_damageStash.Has(request.entity)) return;
                ref var damageComponent = ref _damageStash.Get(request.entity);
                var defence = 0;
                if (_defenceStash.Has(request.entity))
                {
                    ref var defenceComponent = ref _defenceStash.Get(request.entity);
                    defence = defenceComponent.currentDefence;
                }

                var damage = Math.Max(1, damageComponent.baseDamage - defence);

            }
        }

        public void Dispose() { }
    }
}