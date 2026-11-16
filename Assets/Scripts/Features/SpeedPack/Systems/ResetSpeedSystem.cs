using Features.SpeedPack.Requests;
using Scellecs.Morpeh;

namespace Features.SpeedPack.Systems
{
    public class ResetSpeedSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _speedFilter;
        
        private Stash<SpeedComponent> _speedStash;
        
        private Request<ResetSpeedRequest> _resetSpeedRequest;

        public void OnAwake()
        {
            _speedFilter = World.Filter.With<SpeedComponent>().Build();
            
            _speedStash = World.GetStash<SpeedComponent>();
            
            _resetSpeedRequest = World.GetRequest<ResetSpeedRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var request in _resetSpeedRequest.Consume())
            {
                foreach (var entity in _speedFilter)
                {
                    ref var speedComp = ref _speedStash.Get(entity);
                    
                    speedComp.currentSpeed = speedComp.maxSpeed;
                }
            }
        }
    
        public void Dispose()
        {
            
        }
    }
}
