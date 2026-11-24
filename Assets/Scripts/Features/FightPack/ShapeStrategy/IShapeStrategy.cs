using System.Collections.Generic;
using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.FightPack.ShapeStrategy
{
    public interface IShapeStrategy
    {
        bool TryAim(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, out CellCoord dir);
        void CollectCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result);
        void CollectRangeCells(GridComponent gridComp, Stash<CellTileComponent> tiles, SpellConfigSo spell, CellCoord from, CellCoord to, CellCoord dir, List<CellCoord> result); // метод только для круговой атаки
        CellCoord GetStartPoint(CellCoord from, CellCoord to); // метод для получения точки, откуда выполняется атака
    }
}
