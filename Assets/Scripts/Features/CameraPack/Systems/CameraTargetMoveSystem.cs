using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PlayerPack;
using Features.TransformPack;
using Features.UnitPack.Components;
using Global;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CameraPack.Systems
{
    public class CameraTargetMoveSystem : ISystem
    {
        public World World { get; set; }

        private readonly Transform _corridorStart;
        private readonly Transform _corridorEnd;

        private Filter _playerFilter;
        private Filter _playerTargetFilter;
        private Filter _gridFilter;

        private Stash<PlayerPositionComponent> _positionStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<GridComponent> _gridStash;
        private Stash<UnitComponent> _unitStash;

        private Vector3 _homePosition;
        private Quaternion _homeRotation;
        private bool _homeCached;

        public CameraTargetMoveSystem(Transform corridorStart, Transform corridorEnd)
        {
            _corridorStart = corridorStart;
            _corridorEnd = corridorEnd;
        }

        public void OnAwake()
        {
            _playerFilter = World.Filter.With<PlayerTag>().With<PlayerPositionComponent>().Build();
            _playerTargetFilter = World.Filter.With<PlayerTargetTag>().With<TransformComponent>().Build();
            _gridFilter = World.Filter.With<GridComponent>().Build();

            _positionStash = World.GetStash<PlayerPositionComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _unitStash = World.GetStash<UnitComponent>();
        }

        public void OnUpdate(float deltaTime) // todo переделать под активного героя
        {
            if (_playerFilter.IsEmpty() || _playerTargetFilter.IsEmpty()) return;

            var target = _transformStash.Get(_playerTargetFilter.First()).value;

            if (!_homeCached)
            {
                _homePosition = target.position;
                _homeRotation = target.rotation;
                _homeCached = true;
            }

            ref var positionComponent = ref _positionStash.Get(_playerFilter.First());

            if (!positionComponent.positionState)
            {
                target.SetPositionAndRotation(_homePosition, _homeRotation);
                return;
            }

            if (!TryGetNearestPlayerCell(out var nearestDepth)) return;

            ref var grid = ref _gridStash.Get(_gridFilter.First());

            var t = Mathf.Clamp01((nearestDepth + 2 * grid.gridRadius) / (4f * grid.gridRadius));
            positionComponent.progress = t;

            t = Mathf.SmoothStep(0f, 1f, t);

            var position = Vector3.Lerp(_corridorStart.position, _corridorEnd.position, t);
            var rotation = Quaternion.Slerp(_corridorStart.rotation, _corridorEnd.rotation, t);
            target.SetPositionAndRotation(position, rotation);
        }

        private bool TryGetNearestPlayerCell(out int nearestDepth)
        {
            ref var grid = ref _gridStash.Get(_gridFilter.First());

            nearestDepth = 0;
            var nearest = new CellCoord();
            var found = false;

            foreach (var pair in grid.takenCells)
            {
                if (!_unitStash.Has(pair.Value)) continue;
                if (_unitStash.Get(pair.Value).teamSide != TeamSide.Player) continue;
                
                var depth = 2 * pair.Key.R + pair.Key.Q;

                if (!found || depth < nearestDepth ||
                    (depth == nearestDepth && pair.Key.R < nearest.R) ||
                    (depth == nearestDepth && pair.Key.R == nearest.R && pair.Key.Q < nearest.Q))
                {
                    nearestDepth = depth;
                    nearest = pair.Key;
                    found = true;
                }
            }

            return found;
        }

        public void Dispose() { }
    }
}
