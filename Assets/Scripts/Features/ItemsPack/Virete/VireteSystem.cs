using Configs;
using Features.HealthPack;
using Features.InputPack.Requests;
using Features.ManaPack;
using Features.QuickTalkPack.Data;
using Features.QuickTalkPack.Events;
using Features.RaycastPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.ItemsPack.Virete
{
    public sealed class VireteSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _filter;
        private Filter _manaFilter;
        private Stash<RaycastComponent> _raycastStash;
        private Event<LmbStartEvent> _lmbStartEvent;
        private Request<ManaChangeRequest> _manaChangeRequest;
        private Stash<ManaComponent> _manaStash;
        private readonly WorldSettingsSo _worldSettings;
        private Event<QuickTalkTriggerEvent> _event;
        private Stash<UnitComponent> _unitStash;
        private Filter _unitsFilter;
        private Stash<HealthComponent>  _healthStash;

        public VireteSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }
        
        public void OnAwake()
        {
            _filter = World.Filter.With<RaycastComponent>().Build();
            _manaFilter = World.Filter.With<ManaComponent>().Build();
            _lmbStartEvent = World.GetEvent<LmbStartEvent>();
            _raycastStash = World.GetStash<RaycastComponent>();
            
            _manaChangeRequest = World.GetRequest<ManaChangeRequest>();
            _manaStash = World.GetStash<ManaComponent>();
            _event = World.GetEvent<QuickTalkTriggerEvent>();
            _unitsFilter = World.Filter.With<UnitComponent>().With<HealthComponent>().Build();
            _unitStash = World.GetStash<UnitComponent>();
            _healthStash = World.GetStash<HealthComponent>();

            // может просто сделать что начинается quick talk c вирете и там выбор как-то
            // или пока просто по простым правилам давать штуки
            // типа если персонаж мертв - возвращает его к жизни
            // если общее хп меньше половины от максимального - лечит до половины обоих персов
            // если все хорошо то просто 2 урона каждому врагу
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _lmbStartEvent.publishedChanges)
            {
                if (_filter.IsEmpty() && _unitsFilter.IsEmpty()) return;
                ref var raycastComponent = ref _raycastStash.Get(_filter.First());
                if (!raycastComponent.hasHit) continue;

                if (!raycastComponent.hitCollider.gameObject.GetComponent<VireteView>()) continue;
                
                ref var manaComp = ref _manaStash.Get(_manaFilter.First());
                //
                // if (manaComp.currentMana < _worldSettings.vireteManaCost)
                // {
                //     // todo сделать какой-то сигнал
                //     Debug.Log("Недостаточно маны");
                //     continue;
                // }
                // _manaChangeRequest.Publish(new ManaChangeRequest
                // {
                //     amount = _worldSettings.vireteManaCost
                // });
                
                // todo логика выбор и Quick talk
                int health = 0;
                int maxHealth = 0;
                foreach (var entity in _unitsFilter)
                {
                    ref var unitComp =  ref _unitStash.Get(entity);
                    ref var healthComp = ref _healthStash.Get(entity);
                    if (unitComp.teamSide == TeamSide.Player)
                    {
                        health += healthComp.currentHealth;
                        maxHealth += healthComp.currentHealth;
                    }
                }

                QuickTalkTrigger trigger;
                
                if (maxHealth / 2 > health)
                {
                    trigger =  QuickTalkTrigger.VireteHeal;
                    // todo чтобы сам эффект делался
                }
                else
                {
                    trigger = QuickTalkTrigger.VireteDamage;
                    // todo эффект
                }
                _event.NextFrame(new QuickTalkTriggerEvent 
                {
                    trigger = trigger
                });
            }
        }

        public void Dispose() { }
    }
}