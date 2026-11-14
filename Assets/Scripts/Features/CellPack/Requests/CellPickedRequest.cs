using Features.HexGridPack.Data;
using Scellecs.Morpeh;

namespace Features.PickCellPack.Requests
{
    public struct CellPickedRequest : IRequestData
    {
        public CellCoord cellCoord;
    }
}
