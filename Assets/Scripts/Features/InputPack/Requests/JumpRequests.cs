using Scellecs.Morpeh;

namespace Features.InputPack.Requests
{
    public struct JumpStartRequest : IRequestData
    {
        public Entity entity;
    }

    public struct JumpEndRequest : IRequestData
    {
        public Entity entity;
    }
}