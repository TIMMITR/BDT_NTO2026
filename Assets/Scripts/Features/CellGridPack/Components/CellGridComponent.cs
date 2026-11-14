using System.Collections.Generic;
using Features.HexGridPack.Data;
using Scellecs.Morpeh;
using UnityEngine;

namespace Features.CellGridPack.Components
{
    public struct CellGridComponent : IComponent
    {
        public Transform root;
        public float hexRadius;
        public int gridRadius;
        
        public Dictionary<CellCoord, Entity> cells;
        public Dictionary<CellCoord, Entity> takenCells; // todo можно будет попробовать разделить клетки занимаемые игроком и врагом
 
        public Vector3 ToWorldCoord(CellCoord coord) =>
            root.position + coord.ToLocalPosition(hexRadius);
        
        public CellCoord FromWorldCoord(Vector3 world) =>
            CellCoord.FromLocalPosition(world - root.position, hexRadius);
    }
}