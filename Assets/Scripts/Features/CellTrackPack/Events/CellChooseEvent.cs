using Configs;
using Scellecs.Morpeh;

namespace Features.CellTrackPack.Events
{
    public struct CellChooseEvent : IEventData
    {
        public TeamSide cellSide;
        
        public CellChooseEvent(TeamSide cellSide)
        {
            this.cellSide = cellSide;
        }
    }
}
