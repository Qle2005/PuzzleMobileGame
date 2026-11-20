using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Level Data/LevelDataSO")]
public class LevelDataSO : ScriptableObject
{
    [System.Serializable]
    public struct MapObjectPlacement
    {
        public OnMapObject mapObject;
        public Vector2Int originPos;
    }

    
    [System.Serializable]
    public struct StartEndpoint
    {
        public MapObjectPlacement startPoint;
        public MapObjectPlacement endPoint;
    }

    [Header("Thông tin Level")]
    public int levelIndex;
    public string levelName;

    [Header("Cấu hình Level")]
    public List<StartEndpoint> startEndpoints = new List<StartEndpoint>();
    public List<MapObjectPlacement> objectPlacements = new List<MapObjectPlacement>();
    public List<DraggableObject> draggableObjects = new List<DraggableObject>();
    public int Budget;

}
