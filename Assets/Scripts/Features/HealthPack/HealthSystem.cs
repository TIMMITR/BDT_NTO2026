using System;
using Scellecs.Morpeh;

namespace Features.HealthPack
{
    public class HealthSystem : ISystem
    {
        public World World { get; set; }

        private Stash<HealthComponent> _healthStash;
        
        private Request<HealthGiveRequest> _healthGiveRequest;
        private Request<HealthTakeRequest> _healthTakeRequest;
        
        private Event<HealthChangeEvent> _healthChangeEvent;
 
        public void OnAwake()
        {
            foreach (var giveRequest in _healthGiveRequest.Consume())
            {
                if (!_healthStash.Has(giveRequest.entity)) return;
                
                ref var healthComponent = ref _healthStash.Get(giveRequest.entity);

                healthComponent.currentHealth = Math.Min(healthComponent.currentHealth + giveRequest.amount,
                    healthComponent.maxHealth);
                
                _healthChangeEvent.NextFrame(new HealthChangeEvent
                {
                    entity = giveRequest.entity, newAmount =  healthComponent.currentHealth
                });
            }

            foreach (var takeRequest in _healthTakeRequest.Consume())
            {
                if (!_healthStash.Has(takeRequest.entity)) return;
                ref var healthComponent = ref _healthStash.Get(takeRequest.entity);

                if (healthComponent.currentHealth > takeRequest.amount)
                {
                    healthComponent.currentHealth -= takeRequest.amount;
                }
                else
                {
                    healthComponent.currentHealth = 0;
                }

                _healthChangeEvent.NextFrame(new HealthChangeEvent
                {
                    entity = takeRequest.entity, newAmount = healthComponent.currentHealth
                });
            }
        }
 
    
        public void OnUpdate(float deltaTime){

        }

        public void Dispose() {}
    }
}
