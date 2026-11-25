using System.Collections.Generic;
using Configs;
using Features.CalcPack;
using Features.FightPack.Components;
using Features.GridPack;
using Features.GridPack.Data;
using Features.UnitPack.Requests;
using Scellecs.Morpeh;

namespace Features.EnemyAIPack.ActionStrategies
{
    public class AttackStrategy : ActionStrategy
    {
        public AttackStrategy(float weight, float decrease) : base(weight, decrease)
        {
            
        }

        public override bool TryExecute(EnemyContext context)
        {
            var unit = context.unit;
            var from = context.positionStash.Get(unit).coord;
 
            if (!TryFindPlayer(context, from, out var target))
                return false;
            if (!context.spellStash.Has(unit))
                return false;
 
            var spell = context.spellStash.Get(unit).spellData;
            
            if (spell == null)
                return false;
 
            if (from.DistanceTo(target) <= spell.range ||
                MoveToTarget(context, unit, from, target).DistanceTo(target) <= spell.range)
                context.attackDesireStash.Set(unit, new AttackDesireComponent(target, spell));
 
            return true;
        }
        
        private bool TryFindPlayer(EnemyContext ctx, CellCoord from, out CellCoord target)
        {
            var champion = int.MaxValue;
            target = default;
 
            foreach (var unit in ctx.unitFilter)
            {
                if (ctx.unitStash.Get(unit).teamSide != TeamSide.Player)
                    continue;
 
                var coord = ctx.positionStash.Get(unit).coord;
                var dist = from.DistanceTo(coord);
 
                if (dist < champion)
                {
                    champion = dist;
                    target = coord;
                }
            }
 
            return champion < int.MaxValue;
        }
        
        // Возвразает последнюю клетку пути
        private CellCoord MoveToTarget(EnemyContext context, Entity unit, CellCoord from, CellCoord targetCoord)
        {
            var speed = context.speedStash.Get(unit).currentSpeed;
            if (speed <= 0)
                return from;
 
            List<CellCoord> resultPath = null;
 
            foreach (var neighbour in HexMath.GetNeighbours(targetCoord))
            {
                if (!GridData.IsFreeCell(context.grid.Comp, context.cellTileStash, neighbour))
                    continue;
 
                var path = context.BuildPath(from, neighbour, speed);
 
                if (path != null && (resultPath == null || path.Count < resultPath.Count))
                    resultPath = path;
            }
 
            if (resultPath == null)
                return from;
 
            context.moveRequest.Publish(new MoveUnitRequest(unit, resultPath), allowNextFrame: true);
            return resultPath[^1];
        }
    }
}
