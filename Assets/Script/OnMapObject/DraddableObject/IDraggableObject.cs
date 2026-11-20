using UnityEngine;

public interface IDraggableObject:IOnMapRoad
{
    public void HandleDragStart(Vector3 position);
    public void HandleDragUpdate(Vector3 position);
    public void HandleDragEnd(Vector3 position);
}
