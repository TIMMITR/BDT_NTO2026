using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.CellTrackPack.Events
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
