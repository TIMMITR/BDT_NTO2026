using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Requests
{
    public struct CellPickedRequest : IRequestData
    {
        public CellCoord cellCoord;
    }
}
