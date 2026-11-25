using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Tags;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PlayerPack;
using Features.TransformPack;
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

        private Singletone<PlayerSelectionTag> _selection;
        private Singletone<GridComponent> _grid;

        private Stash<PlayerPositionComponent> _positionStash;
        private Stash<TransformComponent> _transformStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<CellPositionComponent> _cellPositionStash;

        private Vector3 _homePosition;
        private Quaternion _homeRotation;
        private bool _homeCached;
        private bool _init;
        private CellCoord _prevCellCoord;

        public CameraTargetMoveSystem(Transform corridorStart, Transform corridorEnd)
        {
            _corridorStart = corridorStart;
            _corridorEnd = corridorEnd;
        }

        public void OnAwake()
        {
            _playerFilter = World.Filter.
                With<PlayerTag>().
                With<PlayerPositionComponent>().
                Build();
            
            _playerTargetFilter = World.Filter.
                With<PlayerTargetTag>().
                With<TransformComponent>().
                Build();

            _selection = new Singletone<PlayerSelectionTag>(World);
            _grid = new Singletone<GridComponent>(World);

            _positionStash = World.GetStash<PlayerPositionComponent>();
            _transformStash = World.GetStash<TransformComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _cellPositionStash = World.GetStash<CellPositionComponent>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (_playerFilter.IsEmpty() || _playerTargetFilter.IsEmpty()) return;

            var target = _transformStash.Get(_playerTargetFilter.First()).value;

            if (!_homeCached)
            {
                _homePosition = target.position;
                _homeRotation = target.rotation;
                _homeCached = true;
                _init = false;
            }

            ref var positionComponent = ref _positionStash.Get(_playerFilter.First());

            if (!positionComponent.positionState)
            {
                target.SetPositionAndRotation(_homePosition, _homeRotation);
                return;
            }

            var selection = _selection.Entity;

            CellCoord chosenCoord;
            if (_chosenCellStash.Has(selection) && _init)
            {
                chosenCoord = _chosenCellStash.Get(selection).cellCoord;
                _prevCellCoord = chosenCoord;
            }
            else if (!_chosenCellStash.Has(selection) && _init)
            {
                chosenCoord = _prevCellCoord;
            }
            else
            {
                chosenCoord = new CellCoord(0, 0);
                _init = true;
            }

            var focusDepth = (float)(2 * chosenCoord.R + chosenCoord.Q);

            if (_selectedUnitStash.Has(selection))
            {
                ref var selected = ref _selectedUnitStash.Get(selection);

                if (_cellPositionStash.Has(selected.unit))
                {
                    var unitCoord = _cellPositionStash.Get(selected.unit).coord;
                    var unitDepth = 2 * unitCoord.R + unitCoord.Q;
                    focusDepth = (focusDepth + unitDepth) * 0.5f;
                }
            }

            var t = Mathf.Clamp01(0.5f + focusDepth / (4f * _grid.Comp.gridRadius));
            positionComponent.progress = t;

            t = Mathf.SmoothStep(0f, 1f, t);

            var position = Vector3.Lerp(_corridorStart.position, _corridorEnd.position, t);
            var rotation = Quaternion.Slerp(_corridorStart.rotation, _corridorEnd.rotation, t);
            target.SetPositionAndRotation(position, rotation);
        }

        public void Dispose() { }
    }
}
