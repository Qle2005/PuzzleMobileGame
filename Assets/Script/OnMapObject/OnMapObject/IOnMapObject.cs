using System.Collections.Generic;
using UnityEngine;

public interface IOnMapObject
{
    public Vector2Int OriginCell { get; set; }
    public List<Vector2Int> OccupiedList { get; }
    public Quaternion Rotation { get; }            
    public List<Cell> OccupiedCells { get; }
   
}
