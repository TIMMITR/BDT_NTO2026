using Features.CameraPack.Components;
using Features.InputPack;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.RaycastPack
{
    public sealed class RaycastSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _playerFilter;
        private Filter _cameraFilter;
        private Filter _raycastFilter;

        private Stash<CameraComponent> _cameraStash;
        private Stash<InputComponent> _inputStash;
        private Stash<RaycastComponent> _raycastStash;

        private const float MaxDistance = 1000f; // todo вынести в найстройки

        public void OnAwake()
        {
            _playerFilter = World.Filter.With<InputComponent>().With<PlayerTag>().Build();
            _cameraFilter = World.Filter.With<CameraComponent>().With<CameraTag>().Build();
            _cameraStash = World.GetStash<CameraComponent>();

            _raycastFilter = World.Filter.With<RaycastComponent>().Build();
            _inputStash = World.GetStash<InputComponent>();
            _raycastStash = World.GetStash<RaycastComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_cameraFilter.IsEmpty() || _raycastFilter.IsEmpty() || _playerFilter.IsEmpty()) return;

            var cameraEntity = _cameraFilter.First();
            ref var camComp = ref _cameraStash.Get(cameraEntity);
            var cam = camComp.value;
            if (!cam) return;
            
            var playerEntity = _playerFilter.First();
            ref var inputComp = ref _inputStash.Get(playerEntity);
            
            var raycastEntity = _raycastFilter.First();
            ref var raycastComp = ref _raycastStash.Get(raycastEntity);
            
            var screenPos = new Vector3(inputComp.mousePos.x, inputComp.mousePos.y, 0f);
            var ray = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out var hit, MaxDistance))
            {
                raycastComp.hasHit = true;
                raycastComp.hit = hit;
                raycastComp.hitPoint = hit.point;
                raycastComp.hitCollider = hit.collider;
            }
            else
            {
                raycastComp.Reset();
            }
        }

        public void Dispose() { }
    }
}