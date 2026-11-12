using Features.CameraPack.Components;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CameraPack.Systems
{
    public class CameraSpawnSystem : IInitializer
    {
        public World World { get; set; }

        private readonly Camera _camera;
        
        public CameraSpawnSystem(Camera camera)
        {
            _camera = camera;
        }
        
        public void OnAwake()
        {
            var camera = World.CreateEntity();
            World.GetStash<CameraTag>().Add(camera);
            
            ref var cameraComp = ref World.GetStash<CameraComponent>().Add(camera);
            cameraComp.value = _camera;
        }
        
        public void Dispose() { }
    }
}