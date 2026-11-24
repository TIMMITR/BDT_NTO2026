using System.Collections.Generic;
using Features.CellSelectionPack.Components;
using Features.CellSelectionPack.Requests;
using Features.CellSelectionPack.Tags;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Global;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Systems
{
    public class PathHighlightSystem : ISystem
    {
        public World World { get; set; }

        private Singletone<PlayerSelectionTag> _selection;

        private Stash<ChosenCellComponent> _chosenCellStash;
        private Stash<SelectedUnitComponent> _selectedUnitStash;
        private Stash<PlannedPathComponent> _plannedPathStash;
        private Stash<PlannedRangeComponent> _plannedRangeStash;
        private Stash<PlannedAttackComponent> _plannedAttackStash;
        private Stash<CellPositionComponent> _positionStash;

        private Request<RefreshHighlightRequest> _refreshRequest;
        private Request<ColorCellRequest> _colorCellRequest;
        private Request<ClearColorsRequest> _clearColorsRequest;

        public void OnAwake()
        {
            _selection = new Singletone<PlayerSelectionTag>(World);

            _chosenCellStash = World.GetStash<ChosenCellComponent>();
            _selectedUnitStash = World.GetStash<SelectedUnitComponent>();
            _plannedPathStash = World.GetStash<PlannedPathComponent>();
            _plannedRangeStash = World.GetStash<PlannedRangeComponent>();
            _plannedAttackStash = World.GetStash<PlannedAttackComponent>();
            _positionStash = World.GetStash<CellPositionComponent>();

            _refreshRequest = World.GetRequest<RefreshHighlightRequest>();
            _colorCellRequest = World.GetRequest<ColorCellRequest>();
            _clearColorsRequest = World.GetRequest<ClearColorsRequest>();
        }

        public void OnUpdate(float deltaTime)
        {
            var refresh = false;
            foreach (var request in _refreshRequest.Consume()) 
                refresh = true;

            if (!refresh)
                return;

            var selection = _selection.Entity;

            _clearColorsRequest.Publish(new ClearColorsRequest());

            if (_plannedPathStash.Has(selection))
                ColorCells(_plannedPathStash.Get(selection).path, ColorType.Path);

            if (_plannedAttackStash.Has(selection))
                ColorCells(_plannedAttackStash.Get(selection).cells, ColorType.Attack);

            if (_plannedRangeStash.Has(selection))
                ColorCells(_plannedRangeStash.Get(selection).range, ColorType.Range);
            
            if (_chosenCellStash.Has(selection))
                ColorCell(_chosenCellStash.Get(selection).cellCoord, ColorType.Chosen);

            if (_selectedUnitStash.Has(selection))
            {
                var unit = _selectedUnitStash.Get(selection).unit;

                if (!World.IsDisposed(unit) && _positionStash.Has(unit))
                    ColorCell(_positionStash.Get(unit).coord, ColorType.Clicked);
            }
        }

        private void ColorCells(List<CellCoord> cells, ColorType type)
        {
            foreach (var cell in cells)
                ColorCell(cell, type);
        }

        private void ColorCell(CellCoord cell, ColorType type) =>
            _colorCellRequest.Publish(new ColorCellRequest(cell, type));

        public void Dispose() { }
    }
}