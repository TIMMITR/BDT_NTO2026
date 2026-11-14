using Configs;
using Scellecs.Morpeh;

namespace Features.UnitPack
{
    public struct UnitSpawnRequest : IRequestData
    {
        public int q;
        public int r;
        public UnitConfig unitConfig; 
    }
}