using Configs;
using Features.CellSelectionPack.Components;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.InputPack.Requests;
using Features.SpeedPack;
using Features.UnitPack.Components;
using Scellecs.Morpeh;

namespace Features.UnitPack.Systems
{
    public class MoveCommandSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<GridComponent> _gridStash;
        private Stash<SpeedComponent> _speedStash;
        private Stash<UnitAnimationComponent> _unitAnimationStash;
        private Stash<CellTileComponent> _cellTileStash;
        
        private Request<LmbStartRequest> _lmbRequest;
        
        
        public void OnAwake()
        {
            _gridFilter = World.Filter.With<GridComponent>().Build();
            
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _gridStash = World.GetStash<GridComponent>();
            _speedStash = World.GetStash<SpeedComponent>();
            _unitAnimationStash = World.GetStash<UnitAnimationComponent>();
            _cellTileStash = World.GetStash<CellTileComponent>();
            
            _lmbRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_gridFilter.IsEmpty()) return; 
            
            bool lmbClicked = false;
            
            var gridEntity = _gridFilter.First();
            ref var gridComp =  ref _gridStash.Get(gridEntity);
            
            foreach (var clickRequest in _lmbRequest.Consume())
            {
                lmbClicked = true;
            }
            
            if (!lmbClicked || !_chosenCellStash.Has(gridEntity)) return;

            if (_pickedCellStash.Has(gridEntity))
            {
                ref var pickedCellComp = ref _pickedCellStash.Get(gridEntity);
                var from = pickedCellComp.cellCoord;
                var to = pickedCellComp.points[^1];
                var unit = gridComp.takenCells[from];

                if (!_unitAnimationStash.Has(unit))
                {
                    _unitAnimationStash.Add(unit, new UnitAnimationComponent(pickedCellComp.points));
                
                    gridComp.takenCells.Remove(pickedCellComp.cellCoord);
                    gridComp.takenCells.Add(to, unit);
                    _cellTileStash.Get(gridComp.cells[from]).blocked = false;
                    _cellTileStash.Get(gridComp.cells[to]).blocked = true;
                }
                
                _pickedCellStash.Remove(gridEntity);
            } 
            else
            {
                var coord = _chosenCellStash.Get(gridEntity).cellCoord;
                
                if (!TryGetPlayerUnit(gridComp, coord, out var speed)) return;
                
                _pickedCellStash.Set(gridEntity, new PickedCellComponent(coord, speed));
            }
        }

        private bool TryGetPlayerUnit(GridComponent gridComp, CellCoord cellCoord, out int speed)
        {
            if (!gridComp.takenCells.TryGetValue(cellCoord, out var unitEntity))
            {
                speed = 0;
                return false;
            }
            
            ref var unitComp = ref _unitStash.Get(unitEntity);
            speed = _speedStash.Get(unitEntity).currentSpeed;
            
            return unitComp.teamSide == TeamSide.Player;
        }
    
        public void Dispose() {}
    }
}
