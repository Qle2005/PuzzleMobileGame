using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IOnMapRoad:IOnMapObject
{
    public List<Transform> ForwardWaypoints {get;}
    public List<Transform> BackwardWaypoints {get;}
    public bool HasConnection(Vector2Int localDir);
    public List<Transform> GetCorrectWaypoints(bool isForward);
}
