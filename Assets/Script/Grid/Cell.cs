using UnityEngine;

public class Cell 
{
    public Vector3Int CellIn3int;
    public Vector3 CellInWorldPos;
    public IOnMapObject occupiedObject;
    public bool IsOccupied => occupiedObject != null;

   public Cell(Vector3Int vector3Int,Vector3 position)
    {
        this.CellIn3int=vector3Int;
        this.CellInWorldPos=position;
        this.occupiedObject = null;
    }

   
}
