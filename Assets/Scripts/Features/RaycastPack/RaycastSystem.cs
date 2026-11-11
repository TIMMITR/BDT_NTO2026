using Features.CameraPack.Components;
using Global;
using Scellecs.Morpeh;

namespace Features.RaycastPack
{
    public sealed class RaycastSystem : ISystem
    {
        public World World { get; set; }
        
        // сюда нужна камера
        // нужно ждать реквеста
        private Filter _cameraFilter;

        public void OnAwake()
        {
            _cameraFilter = World.Filter.With<CameraComponent>().With<CameraTag>().Build();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cameraFilter.IsEmpty()) return;
            var cameraEntity = _cameraFilter.First();
        }

        public void Dispose() { }
    }
}