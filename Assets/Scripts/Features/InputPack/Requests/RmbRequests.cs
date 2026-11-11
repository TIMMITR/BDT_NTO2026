using Scellecs.Morpeh;

namespace Features.InputPack.Requests
{
    
    public struct RmbStartRequest : IRequestData
    {
        public Entity entity;
    }
    
    public struct RmbEndRequest : IRequestData
    {
        public Entity entity;
    }
}