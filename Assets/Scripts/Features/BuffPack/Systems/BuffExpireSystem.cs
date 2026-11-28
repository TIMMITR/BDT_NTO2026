using Features.BuffPack.Components;
using Features.BuffPack.Requests;
using Features.DamagePack;
using Features.HealthPack;
using Features.SpeedPack;
using Features.StatePack.Events;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using System;
using Configs;
using Features.StatePack.Components;

namespace Features.BuffPack.Systems
{
    public sealed class BuffExpireSystem : ISystem
    {
        public World World { get; set; }

        private Filter _buffFilter;

        private Stash<BuffComponent> _buffStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<HealthComponent> _healthStash;
        private Stash<DamageComponent> _damageStash;
        private Stash<DefenceComponent> _defenceStash;
        private Stash<SpeedComponent> _speedStash;

        private Event<PhaseChangedEvent> _phaseChangedEvent;
        private Request<HealthChangeRequest> _healthChangeRequest; // не нужен, убери если не используешь

        public void OnAwake()
        {
            _buffFilter = World.Filter.With<BuffComponent>().Build();

            _buffStash = World.GetStash<BuffComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            _damageStash = World.GetStash<DamageComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();
            _speedStash = World.GetStash<SpeedComponent>();

            _phaseChangedEvent = World.GetEvent<PhaseChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var evt in _phaseChangedEvent.publishedChanges)
            {
                if (evt.type != TurnType.Start)
                    continue;

                foreach (var entity in _buffFilter)
                {
                    if (_unitStash.Has(entity) &&
                        _unitStash.Get(entity).teamSide != GetTeamSide(evt.phase))
                        continue;

                    Tick(entity);
                }
            }
        }

        private void Tick(Entity entity)
        {
            var component = _buffStash.Get(entity);
            var buffs = component.buffs;

            for (var i = buffs.Count - 1; i >= 0; i--) // идет с конца т.к. удаление происходит в цикле
            {
                var buff = buffs[i];

                if (buff.remainingTurns == -1)
                    continue;

                buff.remainingTurns--;
                buffs[i] = buff;

                if (buff.remainingTurns > 0)
                    continue;

                Revert(entity, buff.stat, buff.amount);
                buffs.RemoveAt(i);
            }
        }

        private void Revert(Entity entity, BuffStatType stat, int amount)
        {
            BuffStatApplier.Apply(World, entity, stat, -amount);
        }

        private TeamSide GetTeamSide(TurnPhase phase) =>
            phase == TurnPhase.Enemy ? TeamSide.Enemy : TeamSide.Player;

        public void Dispose() { }
    }
}