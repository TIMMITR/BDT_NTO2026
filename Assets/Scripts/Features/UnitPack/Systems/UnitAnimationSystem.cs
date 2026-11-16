using Features.GridPack.Components;
using Features.TransformPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public class UnitAnimationSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _animationFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;
        private Stash<MoveAnimationDataComponent> _animationDataStash;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _animationFilter = World.Filter.With<UnitAnimationComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();
            _animationDataStash = World.GetStash<MoveAnimationDataComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _animationFilter)
            {
                var gridEntity = _gridFilter.First();
                
                ref var animationComp = ref _unitAnimationStash.Get(entity);
                ref var gridComp = ref _gridStash.Get(gridEntity);
                ref var animationDataComp = ref _animationDataStash.Get(gridEntity); 
                
                var points = animationComp.points;
                var tr = _transformStash.Get(entity).value;
                
                var moveDuration = animationDataComp.animationData.moveDuration;
                var jumpHeight = animationDataComp.animationData.jumpHeight;
                var rotateSpeed = animationDataComp.animationData.rotateSpeed;

                var from = gridComp.ToWorldCoord(points[animationComp.index - 1]);
                var to = gridComp.ToWorldCoord(points[animationComp.index]);
                    
                if (animationComp.isTurning)
                {
                    if(!RotateToTarget(tr, to, rotateSpeed, deltaTime))
                        continue;
                        
                    animationComp.isTurning = false;
                }
                    
                animationComp.progress += deltaTime / moveDuration;
                
                MoveUnit(tr, from, to, animationComp.progress, jumpHeight);

                if (animationComp.progress >= 1f)
                {
                    animationComp.progress = 0f;
                    animationComp.index++;
                    animationComp.isTurning = true;

                    if (animationComp.index >= animationComp.points.Count)
                    {
                        // Конец хода
                        _unitAnimationStash.Remove(entity);
                    }
                }
            }
        }

        private bool RotateToTarget(Transform tr, Vector3 to, float rotationSpeed, float deltaTime) // todo можно будет
                                                                                               // поиграться с временем разворота
                                                                                               // + добавить отдельную кнопку для этого
        {
            var dir = to - tr.position;
            dir.y = 0f;
            
            if (dir.magnitude < 0.001f) return true;
            
            var look = Quaternion.LookRotation(dir);
            tr.rotation = Quaternion.RotateTowards(tr.rotation, look, rotationSpeed * deltaTime);
            
            return Quaternion.Angle(tr.rotation, look) < 0.1f;
        }

        private void MoveUnit(Transform tr, Vector3 from, Vector3 to, float progress, float jumpHeight)
        {
            var pos = Vector3.Lerp(from, to, progress);
            
            pos.y += jumpHeight * progress * (1f - progress);
            tr.position = pos;
        }
        
        public void Dispose() {}
    }
}