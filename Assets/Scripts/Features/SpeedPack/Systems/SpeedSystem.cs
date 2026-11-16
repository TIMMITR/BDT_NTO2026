using Features.SpeedPack.Requests;
using Scellecs.Morpeh;

namespace Features.SpeedPack.Systems
{
    public sealed class SpeedSystem : ISystem
    {
        public World World { get; set; }

        private Stash<SpeedComponent> _speedStash;

        private Request<SpeedChangeRequest> _speedChangeRequest;

        private Event<SpeedChangedEvent> _speedChangeEvent;
 
        public void OnAwake()
        {
            _speedStash = World.GetStash<SpeedComponent>();

            _speedChangeRequest = World.GetRequest<SpeedChangeRequest>();

            _speedChangeEvent = World.GetEvent<SpeedChangedEvent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _speedChangeRequest.Consume())
            {
                if (!_speedStash.Has(request.entity)) continue;

                ref var speedComponent = ref _speedStash.Get(request.entity);
                var newAmount = speedComponent.currentSpeed - request.amount;
                var delta = speedComponent.currentSpeed - newAmount;

                speedComponent.currentSpeed = newAmount;

                _speedChangeEvent.NextFrame(new SpeedChangedEvent
                {
                    entity = request.entity,
                    newAmount = newAmount,
                    delta = delta
                });
            }
        }

        public void Dispose() { }
    }
}
