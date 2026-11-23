using System.Collections.Generic;
using Features.GridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.GridPack.Components
{
    public struct GridComponent : IComponent
    {
        public float cellRadius;
        public int gridRadius;
        
        public Dictionary<CellCoord, Entity> cells;
        public Dictionary<CellCoord, Entity> takenCells;

        public GridComponent(float cellRadius, int gridRadius)
        {
            this.cellRadius = cellRadius;
            this.gridRadius = gridRadius;
            cells = new Dictionary<CellCoord, Entity>();
            takenCells = new Dictionary<CellCoord, Entity>();
        }
        
        public Vector3 ToWorldCoord(CellCoord coord) =>
            coord.ToLocalPosition(cellRadius);
        
        public CellCoord FromWorldCoord(Vector3 world) =>
            CellCoord.FromLocalPosition(world, cellRadius);
    }
}