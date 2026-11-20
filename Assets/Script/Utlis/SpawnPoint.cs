using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SpawnPoint : OnMapRoad
{
    [Header("Cài đặt Utlis Object")]
    [SerializeField] private GameObject objectPrefab; 

    private Vector2Int localDirection;
    private OnMapRoad startPoint;
    private OnMapRoad endPoint;
    private IGridService gridManager;

    public void InitializeSpawnPoint(OnMapRoad start, OnMapRoad end, IGridService grid)
    {
        startPoint = start;
        endPoint = end;
        gridManager = grid;
        SpawnUtlisObject();
    }

    public void SpawnUtlisObject()
    {
        if (objectPrefab == null)
        {
            Debug.LogWarning("Chưa gán Prefab Utlis vào SpawnPoint trong Inspector!", this);
        }

        // Lấy danh sách Waypoints từ OnMapObject
        Transform waypoints= null;
        if (base.baseConnectEast||base.baseConnectNorth)
        {
             waypoints = base.ForwardWaypoints[0];
             localDirection = base.baseConnectEast ? Vector2Int.right : Vector2Int.up;
        }
        else
        {
            waypoints = base.BackwardWaypoints[0];
            localDirection = base.baseConnectWest ? Vector2Int.left : Vector2Int.down;
        }
        //tính toán rotation cho object
        float angle = Mathf.Atan2(localDirection.y, localDirection.x) * Mathf.Rad2Deg;
        // Đưa về khoảng [0, 360)
        if (angle < 0) angle += 360f;
        Quaternion rotation= Quaternion.Euler(0f, angle, 0f);

        if (waypoints != null)
        {
            Transform spawnTransform = waypoints;

            // Spawn xe tại vị trí và góc quay của waypoint đầu tiên
            GameObject gameObject=Instantiate(objectPrefab, spawnTransform.position, rotation);
            if(gameObject.TryGetComponent<VehicleController>(out var vehicle))
            {
                vehicle.Initialize(startPoint,endPoint, gridManager);
            }

            Debug.Log($"Đã spawn Utlis object thành công tại waypoint đầu tiên của hướng {localDirection}.");
        }
        else
        {
            Debug.LogWarning($"Spawn game object lỗi");
        }
    }

}
