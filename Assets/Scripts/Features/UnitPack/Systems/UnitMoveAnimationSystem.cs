using Features.GridPack.Components;
using Features.TransformPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public class UnitMoveAnimationSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _moveFilter;
        
        private Stash<GridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<UnitMoveComponent> _moveUnitStash; // todo возможно, стоит разделить на request + comp реализацию
        private Stash<MoveDataComponent> _moveDataStash;
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            _moveFilter = World.Filter.With<UnitMoveComponent>().Build();
            
            _gridStash = World.GetStash<GridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _moveUnitStash = World.GetStash<UnitMoveComponent>();
            _moveDataStash = World.GetStash<MoveDataComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveFilter)
            {
                var gridEntity = _gridFilter.First();
                
                ref var moveUnitComp = ref _moveUnitStash.Get(entity);
                ref var gridComp = ref _gridStash.Get(_gridFilter.First());
                ref var moveDataComp = ref _moveDataStash.Get(gridEntity); 
                
                var points = moveUnitComp.points;
                var tr = _transformStash.Get(entity).value;
                
                var moveDuration = moveDataComp.moveData.moveDuration;
                var jumpHeight = moveDataComp.moveData.jumpHeight;
                var rotateSpeed = moveDataComp.moveData.rotateSpeed;

                var from = gridComp.ToWorldCoord(points[moveUnitComp.index - 1]);
                var to = gridComp.ToWorldCoord(points[moveUnitComp.index]);
                    
                if (moveUnitComp.isTurning)
                {
                    if(!RotateToTarget(tr, to, rotateSpeed, deltaTime))
                        continue;
                        
                    moveUnitComp.isTurning = false;
                }
                    
                moveUnitComp.percent += deltaTime / moveDuration;
                
                MoveUnit(tr, from, to, moveUnitComp.percent, jumpHeight);

                if (moveUnitComp.percent >= 1f)
                {
                    moveUnitComp.percent = 0f;
                    moveUnitComp.index++;
                    moveUnitComp.isTurning = true;

                    if (moveUnitComp.index >= moveUnitComp.points.Count)
                    {
                        // Конец хода
                        _moveUnitStash.Remove(entity);
                    }
                }
            }
        }

        private bool RotateToTarget(Transform tr, Vector3 to, float duration, float deltaTime) // todo можно будет
                                                                                               // поиграться с временем разворота
                                                                                               // + добавить отдельную кнопку для этого
        {
            var dir = to - tr.position;
            dir.y = 0f;
            
            if (dir.magnitude < 0.001f) return true;
            
            var look = Quaternion.LookRotation(dir);
            tr.rotation = Quaternion.RotateTowards(tr.rotation, look, duration * deltaTime);
            
            return Quaternion.Angle(tr.rotation, look) < 0.1f;
        }

        private void MoveUnit(Transform tr, Vector3 from, Vector3 to, float time, float jumpHeight)
        {
            var pos = Vector3.Lerp(from, to, time);
            
            pos.y += jumpHeight * time * (1f - time);
            tr.position = pos;
        }
        
        public void Dispose() {}
    }
}