using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellSelectionPack.Events
{
    public struct CellChooseChangedEvent : IEventData
    {
        public CellCoord cellCoord;
        
        public CellChooseChangedEvent(CellCoord cellCoord)
        {
            this.cellCoord = cellCoord;
        }
    }
}
