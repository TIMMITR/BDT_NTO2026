using Scellecs.Morpeh;

namespace Features.FightPack.Requests
{
    public struct StartFightMoveRequest : IRequestData
    {
        public Entity target;
        
        public StartFightMoveRequest(Entity target)
        {
            this.target = target;
        }
    }
}
