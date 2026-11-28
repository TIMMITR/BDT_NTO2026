using System.Collections.Generic;
using Features.BuffPack.Requests;
using Scellecs.Morpeh;

namespace Features.BuffPack.Components
{
    public struct BuffComponent : IComponent
    {
        public List<ActiveBuff> buffs;
        
        public BuffComponent(ActiveBuff buff)
        {
            buffs = new List<ActiveBuff>();
            buffs.Add(buff);
        }
    }
 
    public struct ActiveBuff
    {
        public BuffStatType stat;
        public int amount;
        public int remainingTurns;
    }
}
