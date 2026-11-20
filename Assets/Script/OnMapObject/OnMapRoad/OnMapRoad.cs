using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OnMapRoad : OnMapObject, IRotable, IOnMapRoad
{
    [Header("Danh sách điểm mốc xe chạy qua")]
    [SerializeField] List<Transform> forwardWaypoints;
    [SerializeField] List<Transform> backwardWaypoints;
    [Header("Cổng kết nối gốc (khi Rotation = 0)")]
    [SerializeField] protected bool baseConnectNorth;
    [SerializeField] protected bool baseConnectSouth;
    [SerializeField] protected bool baseConnectEast;
    [SerializeField] protected bool baseConnectWest;
    public List<Transform> ForwardWaypoints => forwardWaypoints;

    public List<Transform> BackwardWaypoints => backwardWaypoints;


    public bool HasConnection(Vector2Int localDir)
    {
        int angle = Mathf.RoundToInt(transform.eulerAngles.y) % 360;
        int steps = angle / 90;
        Vector2Int rotatedDir = localDir;
        
        for (int i = 0; i < steps; i++)
        {
            rotatedDir = new Vector2Int(-rotatedDir.y, rotatedDir.x);
        }

        if (rotatedDir == Vector2Int.up) return baseConnectNorth;
        if (rotatedDir == Vector2Int.down) return baseConnectSouth;
        if (rotatedDir == Vector2Int.right) return baseConnectEast;
        if (rotatedDir == Vector2Int.left) return baseConnectWest;

        return false;
    }

    public void Rotate()
    {
        transform.Rotate(0f, 90f, 0f);
    }

    public List<Transform> GetCorrectWaypoints(bool isForward)
    {
        // Lấy góc quay tuyệt đối hiện tại (0, 90, 180, 270)
        int angle = Mathf.RoundToInt(transform.eulerAngles.y) % 360;
        if (angle < 0) angle += 360;

        // Xác định xem ở góc quay này, chiều Forward/Backward có bị đảo ngược hay không
        // (Dựa trên cách đoạn đường của bạn bị lật hướng trong không gian 3D)
        bool isFlipped = (angle == 180 || angle == 270); // Hoặc điều chỉnh tùy theo thiết kế prefab của bạn

        if (isFlipped)
        {
            // Nếu bị lật ngược, đảo chiều lấy waypoint
            return isForward ? backwardWaypoints : forwardWaypoints;
        }
        else
        {
            // Giữ nguyên chiều
            return isForward ? forwardWaypoints : backwardWaypoints;
        }
    }
}
