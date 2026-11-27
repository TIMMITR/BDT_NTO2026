using Configs;
using Features.GridPack.Data;
using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct UnitSpawnRequest : IRequestData
    {
        public CellCoord cellCoord;
        public UnitConfig unitConfig;
        public bool isBlocked;
        public bool isBase;

        public UnitSpawnRequest(CellCoord cellCoord, UnitConfig unitConfig, bool isBlocked, bool isBase)
        {
            this.cellCoord = cellCoord;
            this.unitConfig = unitConfig;
            this.isBlocked = isBlocked;
            this.isBase = isBase;
        }
    }
}