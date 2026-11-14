using System.Collections.Generic;
using Configs;
using Scellecs.Morpeh;
using UnityEngine.TextCore.Text;

namespace Features.UnitPack.Components
{
    public struct UnitsDataComponent : IComponent
    {
        public Dictionary<UnitClass, Entity> unitsData;
    }
}
