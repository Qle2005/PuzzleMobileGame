using UnityEngine;

public interface IGridService 
{
    public Cell GetCell (Vector2Int vector2Int);
    public Cell WorldPosToCell (Vector3 position);
    public Vector3 CellToWorldPos(Vector2Int originPos);
    public void PlaceObject(IOnMapObject draggableObject, Vector2Int cellPos);
    public void ClearObjectFromGrid(IOnMapObject draggableObject);
    public bool CanPlaceObject(IOnMapObject draggableObject,Vector2Int cellPos);
    public bool TrySnapToCell(Transform transform,out Vector2Int cellPos);
}
