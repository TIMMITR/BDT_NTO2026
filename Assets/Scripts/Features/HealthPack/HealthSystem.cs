using System;
using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public sealed class HealthSystem : ISystem
    {
        public World World { get; set; }

        private Stash<HealthComponent> _healthStash;
        private Stash<DefenceComponent> _defenceStash;

        private Request<HealthRequest> _healthChangeRequest;

        private Event<HealthEvent> _healthChangeEvent;
 
        public void OnAwake()
        {
            _healthStash = World.GetStash<HealthComponent>();
            _defenceStash = World.GetStash<DefenceComponent>();

            _healthChangeRequest = World.GetRequest<HealthRequest>();

            _healthChangeEvent = World.GetEvent<HealthEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _healthChangeRequest.Consume())
            {
                if (!_healthStash.Has(request.entity)) continue;

                ref var healthComponent = ref _healthStash.Get(request.entity);

                var amount = request.amount;
                if (amount < 0 && _defenceStash.Has(request.entity))
                {
                    ref var defenceComponent = ref _defenceStash.Get(request.entity);
                    amount = -Math.Max(1, -amount - defenceComponent.currentDefence);
                }

                var newHealth = Math.Clamp(healthComponent.currentHealth + amount, 0, healthComponent.maxHealth);
                var delta = newHealth - healthComponent.currentHealth;
                healthComponent.currentHealth = newHealth;

                _healthChangeEvent.NextFrame(new HealthEvent
                {
                    entity = request.entity,
                    newAmount = newHealth,
                    delta = delta
                });
            }
        }

        public void Dispose() { }
    }
}
