using System;
using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public sealed class ManaSystem : ISystem
    {
        public World World { get; set; }

        private Stash<ManaComponent> _manaStash;
        
        private Request<ManaGiveRequest> _manaGiveRequest;
        private Request<ManaTakeRequest> _manaTakeRequest;
        
        private Event<ManaChangeSuccessEvent> _manaChangeSuccessEvent;
        private Event<ManaChangeFailedEvent> _manaChangeFailedEvent;

        public void OnAwake()
        {
            _manaStash = World.GetStash<ManaComponent>();
            
            _manaGiveRequest = World.GetRequest<ManaGiveRequest>();
            _manaTakeRequest = World.GetRequest<ManaTakeRequest>();
            
            _manaChangeSuccessEvent = World.GetEvent<ManaChangeSuccessEvent>();
            _manaChangeFailedEvent = World.GetEvent<ManaChangeFailedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var giveRequest in _manaGiveRequest.Consume())
            {
                if (!_manaStash.Has(giveRequest.entity)) return;
                
                ref var manaComponent = ref _manaStash.Get(giveRequest.entity);

                manaComponent.currentMana = Math.Min(manaComponent.currentMana + giveRequest.amount, manaComponent.maxMana);
                
                _manaChangeSuccessEvent.NextFrame(new ManaChangeSuccessEvent{newAmount = manaComponent.currentMana});
                
            }
            
            foreach (var takeRequest in _manaTakeRequest.Consume())
            {
                if (!_manaStash.Has(takeRequest.entity)) return;
                
                ref var manaComponent = ref _manaStash.Get(takeRequest.entity);
                
                if (manaComponent.currentMana >= takeRequest.amount)
                {
                    manaComponent.currentMana -= takeRequest.amount;
                    _manaChangeSuccessEvent.NextFrame(new ManaChangeSuccessEvent{newAmount = manaComponent.currentMana});
                }
                else
                {
                    _manaChangeFailedEvent.NextFrame(new ManaChangeFailedEvent());
                }
                
            }
        }

        public void Dispose() { }
    }
}