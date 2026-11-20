using Configs;
using Scellecs.Morpeh;

namespace Features.UnitPack.Requests
{
    public struct UnitSpawnRequest : IRequestData
    {
        public int q;
        public int r;
        public UnitConfig unitConfig;
        public bool isBlocked;

        public UnitSpawnRequest(int q, int r, UnitConfig unitConfig, bool isBlocked)
        {
            this.q = q;
            this.r = r;
            this.unitConfig = unitConfig;
            this.isBlocked = isBlocked;
        }
    }
}