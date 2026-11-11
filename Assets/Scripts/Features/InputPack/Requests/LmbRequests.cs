using Scellecs.Morpeh;

namespace Features.InputPack.Requests
{
    public struct LmbStartRequest : IRequestData
    {
        public Entity entity;
    }

    public struct LmbEndRequest : IRequestData
    {
        public Entity entity;
    }

}