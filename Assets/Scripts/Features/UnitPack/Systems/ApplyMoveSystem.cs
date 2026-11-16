using Features.CellGridPack.Components;
using Features.TransformPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public class ApplyMoveSystem : ISystem // todo переименовать
    {
        public World World { get; set; }

        private Filter _gridFilter;
        private Filter _moveFilter;
        
        private Stash<CellGridComponent> _gridStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<UnitMoveComponent> _moveStash; // todo возможно, стоит разделить на request + comp реализацию
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            _moveFilter = World.Filter.With<UnitMoveComponent>().Build();
            
            _gridStash = World.GetStash<CellGridComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _moveStash = World.GetStash<UnitMoveComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            foreach (var entity in _moveFilter)
            {
                ref var moveComp = ref _moveStash.Get(entity);
                ref var gridComp = ref _gridStash.Get(_gridFilter.First());
                
                var points = moveComp.points;
                var tr = _transformStash.Get(entity).value;
                var duration = moveComp.duration;

                var from = gridComp.ToWorldCoord(points[moveComp.index - 1]);
                var to = gridComp.ToWorldCoord(points[moveComp.index]);
                    
                if (moveComp.isTurning)
                {
                    if(!RotateToTarget(tr, to, 360f, deltaTime))
                        continue;
                        
                    moveComp.isTurning = false;
                }
                    
                moveComp.percent += deltaTime / moveComp.duration;
                
                MoveUnit(tr, from, to, moveComp.percent, moveComp.jumpHeight);

                if (moveComp.percent >= 1f)
                {
                    moveComp.percent = 0f;
                    moveComp.index++;
                    moveComp.isTurning = true;

                    if (moveComp.index >= moveComp.points.Count)
                    {
                        // Конец хода
                        _moveStash.Remove(entity);
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
            
            pos.y += jumpHeight * 5f * time * (1f - time);
            tr.position = pos;
        }
        
        public void Dispose() { }
    }
}
