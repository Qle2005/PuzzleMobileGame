using System.Collections.Generic;
using UnityEngine;

public class RoadAStarNode
{
    public IOnMapRoad IOnMapRoad;
    public RoadAStarNode parent;
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost;

    public RoadAStarNode(IOnMapRoad IOnMapRoad)
    {
        this.IOnMapRoad = IOnMapRoad;
    }
}

public class RoadPathFinder
{
    public static List<Transform> FindPath(IOnMapRoad startRoad, IOnMapRoad endRoad, IGridService gridManager)
    {
        Debug.Log($"Đang tìm kiếm đường đi từ {startRoad.OriginCell} đến {endRoad.OriginCell}");
        if (startRoad == null || endRoad == null) return null;

        List<RoadAStarNode> openSet = new List<RoadAStarNode>();
        HashSet<IOnMapRoad> closedSet = new HashSet<IOnMapRoad>();

        RoadAStarNode startNode = new RoadAStarNode(startRoad);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            // Lấy node có fCost nhỏ nhất
            RoadAStarNode currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.IOnMapRoad);

            // Nếu đã đi đến đoạn đường đích
            if (currentNode.IOnMapRoad == endRoad)
            {
                return RetraceWaypoints(currentNode);
            }

            // Duyệt các đoạn đường lân cận
            foreach (var neighborRoad in RoadNetworkHelper.GetConnectedRoads(currentNode.IOnMapRoad, gridManager))
            {
                if (closedSet.Contains(neighborRoad)) continue;

                float tentativeGCost = currentNode.gCost + 1f; // Chi phí qua mỗi đoạn đường tính là 1 bước

                RoadAStarNode neighborNode = openSet.Find(n => n.IOnMapRoad == neighborRoad);

                if (neighborNode == null)
                {
                    neighborNode = new RoadAStarNode(neighborRoad);
                    openSet.Add(neighborNode);
                }
                else if (tentativeGCost >= neighborNode.gCost)
                {
                    continue;
                }

                neighborNode.parent = currentNode;
                neighborNode.gCost = tentativeGCost;
                
                // Tính khoảng cách heuristic dựa trên vị trí OriginCell của các đoạn đường
                neighborNode.hCost = Vector2Int.Distance(neighborRoad.OriginCell, endRoad.OriginCell);
            }
        }

        return null; // Không tìm thấy đường đi
    }

    private static List<Transform> RetraceWaypoints(RoadAStarNode endNode)
    {
        List<IOnMapRoad> roadPath = new List<IOnMapRoad>();
        RoadAStarNode currentNode = endNode;

        while (currentNode != null)
        {
            roadPath.Add(currentNode.IOnMapRoad);
            currentNode = currentNode.parent;
        }
        roadPath.Reverse();

        // Gom toàn bộ mảng waypoints của các đoạn đường theo đúng thứ tự A* tìm được
        List<Transform> finalWaypoints = RetraceWaypointsWithTwoSets(roadPath);

        return finalWaypoints;
    }

    public static List<Transform> RetraceWaypointsWithTwoSets(List<IOnMapRoad> roadPath)
    {
        List<Transform> finalWaypoints = new List<Transform>();

        if (roadPath == null || roadPath.Count == 0) return finalWaypoints;

        for (int i = 0; i < roadPath.Count; i++)
        {
            IOnMapRoad currentRoad = roadPath[i];
            bool isForward = true;

            // Xác định hướng đi (Forward hay Backward) so với đoạn trước hoặc sau
            if (i < roadPath.Count - 1)
            {
                IOnMapRoad nextRoad = roadPath[i + 1];
                isForward = IsMovingForward(currentRoad, nextRoad);
            }
            else if (i > 0)
            {
                IOnMapRoad prevRoad = roadPath[i - 1];
                isForward = IsMovingForward(prevRoad, currentRoad);
            }

            // Lấy danh sách waypoint đã được tự động chuẩn hóa theo góc xoay thực tế của tile
            List<Transform> chosenWaypoints = currentRoad.GetCorrectWaypoints(isForward);
          

            // Thêm vào danh sách tổng
            if (chosenWaypoints != null)
            {
                foreach (var wp in chosenWaypoints)
                {
                    if (wp != null) finalWaypoints.Add(wp);
                }
            }
        }

        return finalWaypoints;
    }

    private static bool IsMovingForward(IOnMapRoad fromRoad, IOnMapRoad toRoad)
    {
        // Lấy hiệu tọa độ OriginCell của 2 đoạn đường
        Vector2Int direction = toRoad.OriginCell - fromRoad.OriginCell;

        // Quy ước: Nếu xe đi theo chiều dương của lưới (lên trên hoặc sang phải), 
        // ta dùng ForwardWaypoints. Ngược lại dùng BackwardWaypoints.
        if (direction.x > 0 || direction.y > 0)
        {
            return true;
        }
        
        return false;
    }
}