using Features.GridPack;
using Features.GridPack.Components;
using Features.SpeedPack.Requests;
using Features.UnitPack.Components;
using Features.UnitPack.Events;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.UnitPack.Systems
{
    public sealed class MoveCommandSystem : ISystem
    {
        public World World { get; set; }

        private Filter _gridFilter;

        private Stash<GridComponent> _gridStash;
        private Stash<CellTileComponent> _tileStash;
        private Stash<CellPositionComponent> _positionStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;

        private Request<MoveUnitRequest> _moveUnitRequest;
        private Request<SpeedChangeRequest> _speedChangeRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();

            _gridStash = World.GetStash<GridComponent>();
            _tileStash = World.GetStash<CellTileComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();

            _moveUnitRequest = World.GetRequest<MoveUnitRequest>();
            _speedChangeRequest = World.GetRequest<SpeedChangeRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_gridFilter.IsEmpty()) return;

            var gridEntity = _gridFilter.First();
            ref var gridComp = ref _gridStash.Get(gridEntity);

            foreach (var request in _moveUnitRequest.Consume())
            {
                var unit = request.unit;
                var path = request.path;

                if (path == null || path.Count < 2) 
                    continue;
                if (_unitAnimationStash.Has(unit)) 
                    continue;

                GridOccupancy.Move(ref gridComp, _tileStash, _positionStash, unit, path[0], path[^1]);
                _unitAnimationStash.Add(unit, new UnitAnimationComponent(path, false));
                var cost = path.Count - 1;
                _speedChangeRequest.Publish(new SpeedChangeRequest(cost, unit));
            }
        }

        public void Dispose() {}
    }
}