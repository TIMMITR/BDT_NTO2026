using Global;
using Scellecs.Morpeh;

namespace Features.TargetPack
{
    public sealed class TargetSystem : ISystem
    {
        public World World { get; set; }

        private Filter _playerFilter;
        private Filter _cameraFilter;
        
        private Stash<TransformComponent> _transformStash;
        
        public void OnAwake()
        {
            _playerFilter = World.Filter.With<TransformComponent>().With<PlayerTargetTag>().Build();
            _cameraFilter = World.Filter.With<TransformComponent>().With<CameraTargetTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_playerFilter.IsEmpty() || _cameraFilter.IsEmpty()) return;
            
            var playerTarget = _playerFilter.First();
            ref var playerTransformComp = ref _transformStash.Get(playerTarget);
            
            var cameraTarget = _cameraFilter.First();
            ref var cameraTransformComp = ref _transformStash.Get(cameraTarget);

            cameraTransformComp.value = playerTransformComp.value;
        }

        public void Dispose() { }
    }
}