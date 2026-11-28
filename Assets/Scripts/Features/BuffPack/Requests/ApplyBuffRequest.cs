using Scellecs.Morpeh;

namespace Features.BuffPack.Requests
{
    public struct ApplyBuffRequest : IRequestData
    {
        public Entity entity;
        public BuffStatType stat;
        public int amount;
        public int duration; // -1 - постоянный бафф
 
        public ApplyBuffRequest(Entity entity, BuffStatType stat, int amount, int duration = -1)
        {
            this.entity = entity;
            this.stat = stat;
            this.amount = amount;
            this.duration = duration;
        }
    }
 
    public enum BuffStatType
    {
        Health,  
        MaxHealth,
        Damage,
        Defence,
        Speed,
        MaxSpeed
    }
}
