using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Requests
{
    public struct ColorCellRequest : IRequestData
    {
        public CellCoord cellCoord;
        public ColorType colorType;

        public ColorCellRequest(CellCoord cellCoord, ColorType colorType)
        {
            this.cellCoord = cellCoord;
            this.colorType = colorType;
        }
    }

    public enum ColorType
    {
        Chosen,
        Clicked,
        Path,
        Attack
    }
}
