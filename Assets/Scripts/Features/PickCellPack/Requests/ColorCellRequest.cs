using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.PickCellPack.Requests
{
    public struct ColorCellRequest : IRequestData
    {
        public CellCoord cellCoord;
    }
}
