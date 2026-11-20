using Scellecs.Morpeh;
using UnityEngine;

namespace Features.PlayerPack
{
    public struct PlayerPositionComponent : IComponent
    {
        [Range(0f, 1f)] public float progress; // нужен для Battle, обозначает как близко к максимому
        public bool positionState; // 0 - стандартный, 1 - баттл
    }
}