using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.Requests
{
    public struct ColorCellRequest : IRequestData
    {
        public CellCoord cellCoord;
    }
}
