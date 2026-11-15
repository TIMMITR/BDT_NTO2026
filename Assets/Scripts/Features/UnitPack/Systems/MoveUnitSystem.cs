using Configs;
using Features.CellGridPack.Components;
using Features.CellPack.Components;
using Features.HexGridPack.Data;
using Features.InputPack.Requests;
using Features.PickCellPack.Requests;
using Features.UnitPack.Components;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.UnitPack.Systems
{
    public class MoveUnitSystem : ISystem
    {
        public World World { get; set; }
        
        private Filter _gridFilter;
        
        private Stash<PickedCellComponent> _pickedCellStash;
        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<UnitComponent> _unitStash;
        private Stash<CellGridComponent> _gridStash;
        
        private Request<LmbStartRequest> _lmbRequest;

        public void OnAwake()
        {
            _gridFilter = World.Filter.With<CellGridComponent>().Build();
            
            _pickedCellStash = World.GetStash<PickedCellComponent>();
            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _unitStash = World.GetStash<UnitComponent>();
            _gridStash = World.GetStash<CellGridComponent>();
            
            _lmbRequest = World.GetRequest<LmbStartRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            if(_gridFilter.IsEmpty()) return; 
            
            bool lmbClicked = false;
            
            var gridEntity = _gridFilter.First();
            
            foreach (var clickRequest in _lmbRequest.Consume())
            {
                lmbClicked = true;
            }
            
            if (!lmbClicked || !_chosenCellStash.Has(gridEntity)) return;

            if (!_pickedCellStash.IsEmpty())
            {
                Debug.Log("Yeah");
            }
            else
            {
                ref var gridComp = ref _gridStash.Get(gridEntity);
                var coord = _chosenCellStash.Get(gridEntity).cellCoord;
                
                if (!IsPlayerFigure(gridComp, coord)) return;
                
                _pickedCellStash.Set(gridEntity, new PickedCellComponent{cellCoord = coord});
            }
        }

        private bool IsPlayerFigure(CellGridComponent gridComp, CellCoord cellCoord)
        {
            if (!gridComp.takenCells.TryGetValue(cellCoord, out var unitEntity)) 
                return false;
            
            ref var unitComp = ref _unitStash.Get(unitEntity);
            
            return unitComp.teamSide == TeamSide.Player;
        }
    
        public void Dispose() {}
    }
}
