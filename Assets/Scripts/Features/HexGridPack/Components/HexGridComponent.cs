using Scellecs.Morpeh;
using UnityEngine;

namespace Features.HexGridPack.Components
{
    public struct HexGridComponent : IComponent
    {
        public Transform root;
        public float hexRadius;
        public int gridRadius;
    }
}