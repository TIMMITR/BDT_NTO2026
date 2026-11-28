using Features.BuffPack.Components;
using Features.BuffPack.Requests;
using Features.DamagePack;
using Features.HealthPack;
using Features.SpeedPack;
using Scellecs.Morpeh;
using System;

namespace Features.BuffPack.Systems
{
    public sealed class BuffApplySystem : ISystem
    {
        public World World { get; set; }

        private Stash<BuffComponent> _buffStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<DamageComponent> _damageStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<SpeedComponent> _speedStash;

        private Request<ApplyBuffRequest> _applyBuffRequest;
        private Request<HealthChangeRequest> _healthChangeRequest;
        //todo добавить request на speed

        public void OnAwake()
        {
            _buffStash = World.GetStash<BuffComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _speedStash = World.GetStash<SpeedComponent>();

            _applyBuffRequest = World.GetRequest<ApplyBuffRequest>();
            _healthChangeRequest = World.GetRequest<HealthChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _applyBuffRequest.Consume())
            {
                if (World.IsDisposed(request.entity))
                    continue;

                BuffStatApplier.Apply(World, request.entity, request.stat, request.amount);
                
                if (request.stat != BuffStatType.Health) // Health - одноразовое, поэтому хранить не нужно
                    Save(request.entity, request);
            }
        }

        private void Save(Entity entity, ApplyBuffRequest request)
        { 
            var component = _buffStash.Has(entity)
                ? _buffStash.Get(entity)
                : new BuffComponent();

            component.buffs.Add(new ActiveBuff
            {
                stat = request.stat,
                amount = request.amount,
                remainingTurns = request.duration
            });

            _buffStash.Set(entity, component);
        }

        public void Dispose() { }
    }
}