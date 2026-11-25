using Configs;
using Features.GridPack.Components;
using Features.GridPack.Data;
using Features.PathFinderPack;
using Scellecs.Morpeh;

namespace Features.FightPack
{
    public static class AttackPlanner
    {
        public static bool TryFindAttackPos(GridComponent gridComp, Stash<CellTileComponent> tiles, PassReady ready,
            SpellConfigSo spell, CellCoord target, int maxCost, out CellCoord pos, out CellCoord dir)
        {
            pos = default;
            dir = default;
            var bestCost = int.MaxValue;
 
            foreach (var pair in ready.cellsCost)
            {
                if (pair.Value > maxCost || pair.Value >= bestCost)
                    continue;
                if (!SpellArea.TryAim(gridComp, tiles, spell, pair.Key, target, out var aimDir))
                    continue;
 
                pos = pair.Key;
                dir = aimDir;
                bestCost = pair.Value;
            }
 
            return bestCost != int.MaxValue;
        }
    }
}
