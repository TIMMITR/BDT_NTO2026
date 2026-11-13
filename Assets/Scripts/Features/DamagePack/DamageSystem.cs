using Features.HealthPack;
using Scellecs.Morpeh;

namespace Features.DamagePack
{
    public sealed class DamageSystem : ISystem
    {
        public World World { get; set; }

        private Stash<DamageComponent> _damageStash;

        private Request<DamageRequest> _damageRequest;
        private Request<HealthRequest> _healthChangeRequest;

        public void OnAwake()
        {
            _damageStash = World.GetStash<DamageComponent>();

            _damageRequest = World.GetRequest<DamageRequest>();
            _healthChangeRequest = World.GetRequest<HealthRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _damageRequest.Consume())
            {
                if (!_damageStash.Has(request.source)) continue;

                ref var damageComponent = ref _damageStash.Get(request.source);

                _healthChangeRequest.Publish(new HealthRequest
                {
                    entity = request.target,
                    amount = -damageComponent.baseDamage
                });
            }
        }

        public void Dispose() { }
    }
}
