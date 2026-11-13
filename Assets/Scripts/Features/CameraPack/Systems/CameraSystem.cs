using Scellecs.Morpeh;

namespace Features.CameraPack.Systems
{
    public sealed class CameraSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _cameraFilter;
        
        public void OnAwake() // todo если будем делать от первого лица сделать вращение 
        {
        }

        public void OnUpdate(float deltaTime)
        {
        }

        public void Dispose() { }
    }
}