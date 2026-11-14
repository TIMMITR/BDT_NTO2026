using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.LevelPack.Requests
{
    public struct ColorCellRequest : IRequestData
    {
        public CellCoord cellCoord;
    }
}
