using Features.HealthPack;
using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public sealed class DamageSystem : ISystem
    {
        public World World { get; set; }

        private Stash<DamageComponent> _damageStash;

        private Request<DamageRequest> _damageRequest;
        private Request<HealthChangeRequest> _healthChangeRequest;

        public void OnAwake()
        {
            _damageStash = World.GetStash<DamageComponent>();

            _damageRequest = World.GetRequest<DamageRequest>();
            _healthChangeRequest = World.GetRequest<HealthChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _damageRequest.Consume())
            {
                int totalDamage;
                
                if (World.IsDisposed(request.source))
                {
                    totalDamage = -request.amount;
                }
                else
                {
                    ref var damageComponent = ref _damageStash.Get(request.source);
                    totalDamage = -damageComponent.multiplier * request.amount;
                }
                
                _healthChangeRequest.Publish(new HealthChangeRequest
                {
                    entity = request.target,
                    amount = totalDamage
                });
            }
        }

        public void Dispose() { }
    }
}
