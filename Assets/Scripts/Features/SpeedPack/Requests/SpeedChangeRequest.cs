using Scellecs.Morpeh;

namespace Features.SpeedPack.Requests
{
    public struct SpeedChangeRequest : IRequestData
    {
        public int amount;
        public Entity entity;
    }
}
