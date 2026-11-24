using Configs;
using Features.HealthPack;
using Features.InputPack.Requests;
using Features.ManaPack;
using Features.QuickTalkPack.Data;
using Features.QuickTalkPack.Events;
using Features.RaycastPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.ItemsPack.Virete
{
    public sealed class VireteSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _raycastFilter;
        private Filter _manaFilter;
        private Filter _unitsFilter;
        
        private Stash<RaycastComponent> _raycastStash;
        private Stash<HealthComponent>  _healthStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<ManaComponent> _manaStash;
        
        private Request<ManaChangeRequest> _manaChangeRequest;
        
        private Event<LmbStartEvent> _lmbStartEvent;
        private Event<QuickTalkTriggerEvent> _quickTalkTriggerEvent;
        
        private readonly WorldSettingsSo _worldSettings;

        public VireteSystem(WorldSettingsSo worldSettings)
        {
            _worldSettings = worldSettings;
        }
        
        public void OnAwake()
        {
            _raycastFilter = World.Filter.With<RaycastComponent>().Build();
            _manaFilter = World.Filter.With<ManaComponent>().Build();
            _unitsFilter = World.Filter.With<UnitComponent>().With<HealthComponent>().Build();
            
            _raycastStash = World.GetStash<RaycastComponent>();
            _manaStash = World.GetStash<ManaComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _healthStash = World.GetStash<HealthComponent>();
            
            _manaChangeRequest = World.GetRequest<ManaChangeRequest>();
            
            _lmbStartEvent = World.GetEvent<LmbStartEvent>();
            _quickTalkTriggerEvent = World.GetEvent<QuickTalkTriggerEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var _ in _lmbStartEvent.publishedChanges)
            {
                if (_raycastFilter.IsEmpty() && _unitsFilter.IsEmpty()) return;
                ref var raycastComponent = ref _raycastStash.Get(_raycastFilter.First());
                if (!raycastComponent.hasHit) continue;

                if (!raycastComponent.hitCollider.gameObject.GetComponent<VireteView>()) continue;
                
                ref var manaComp = ref _manaStash.Get(_manaFilter.First());
                
                // if (manaComp.currentMana < _worldSettings.vireteManaCost)
                // {
                //     // todo сделать какой-то сигнал
                //     Debug.Log("Недостаточно маны");
                //     continue;
                // }
                
                _manaChangeRequest.Publish(new ManaChangeRequest
                {
                    amount = _worldSettings.vireteManaCost
                });
                
                // todo логика выбор и Quick talk
                int health = 0;
                int maxHealth = 0;
                foreach (var entity in _unitsFilter)
                {
                    ref var unitComp =  ref _unitStash.Get(entity);
                    ref var healthComp = ref _healthStash.Get(entity);
                    if (unitComp.teamSide != TeamSide.Player) continue;
                    health += healthComp.currentHealth;
                    maxHealth += healthComp.currentHealth;
                }

                QuickTalkTrigger trigger;
                
                if (maxHealth / 2 >= health)
                {
                    trigger =  QuickTalkTrigger.VireteHeal;
                    // todo чтобы сам эффект делался
                }
                else
                {
                    trigger = QuickTalkTrigger.VireteDamage;
                    // todo эффект
                }
                _quickTalkTriggerEvent.NextFrame(new QuickTalkTriggerEvent 
                {
                    trigger = trigger
                });
            }
        }

        public void Dispose() { }
    }
}