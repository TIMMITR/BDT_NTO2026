using System;
using Scellecs.Morpeh;

namespace Features.ManaPack
{
    public sealed class ManaSystem : ISystem
    {
        public World World { get; set; }

        private Stash<ManaComponent> _manaStash;

        private Request<ManaChangeRequest> _manaChangeRequest;

        private Event<ManaChangeSuccessEvent> _manaChangeSuccessEvent;
        private Event<ManaChangeFailedEvent> _manaChangeFailedEvent;

        public void OnAwake()
        {
            _manaStash = World.GetStash<ManaComponent>();

            _manaChangeRequest = World.GetRequest<ManaChangeRequest>();

            _manaChangeSuccessEvent = World.GetEvent<ManaChangeSuccessEvent>();
            _manaChangeFailedEvent = World.GetEvent<ManaChangeFailedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _manaChangeRequest.Consume())
            {
                if (!_manaStash.Has(request.entity)) continue;

                ref var manaComponent = ref _manaStash.Get(request.entity);

                if (request.amount < 0 && manaComponent.currentMana < -request.amount)
                {
                    _manaChangeFailedEvent.NextFrame(new ManaChangeFailedEvent { entity = request.entity });
                    continue;
                }

                manaComponent.currentMana = Math.Clamp(manaComponent.currentMana + request.amount, 0, manaComponent.maxMana);

                _manaChangeSuccessEvent.NextFrame(new ManaChangeSuccessEvent
                {
                    entity = request.entity,
                    newAmount = manaComponent.currentMana
                });
            }
        }

        public void Dispose() { }
    }
}
