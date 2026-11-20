using System.Collections.Generic;
using UnityEngine;

public static class RoadNetworkHelper
{
    public static List<IOnMapRoad> GetConnectedRoads(IOnMapRoad currentRoad, IGridService gridManager)
    {
        List<IOnMapRoad> connectedRoads = new List<IOnMapRoad>();
        HashSet<IOnMapRoad> uniqueRoads = new HashSet<IOnMapRoad>();

        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

        // Duyệt qua tất cả các cell mà đoạn đường này đang chiếm giữ
        foreach (var cell in currentRoad.OccupiedCells)
        {
            Vector2Int cellPos = new Vector2Int(cell.CellIn3int.x, cell.CellIn3int.z);

            foreach (var dir in directions)
            {
                // 1. Kiểm tra xem đoạn đường có mở cổng về hướng này không
                if (!currentRoad.HasConnection(dir)) continue;

                Vector2Int neighborPos = cellPos + dir;
                Cell neighborCell = gridManager.GetCell(neighborPos);

                // 2. Nếu ô lân cận có chứa vật thể và vật thể đó là một IOnMapRoad khác
                if (neighborCell != null && neighborCell.IsOccupied)
                {
                    IOnMapRoad neighborRoad = (IOnMapRoad)neighborCell.occupiedObject;

                    // Đảm bảo không phải chính nó và chưa bị trùng lặp trong danh sách
                    if (neighborRoad != null && neighborRoad != currentRoad && !uniqueRoads.Contains(neighborRoad))
                    {
                        // 3. Kiểm tra chiều ngược lại của đoạn đường kế tiếp có mở cổng đón không
                        Vector2Int oppositeDir = -dir;
                        if (neighborRoad.HasConnection(oppositeDir))
                        {
                            uniqueRoads.Add(neighborRoad);
                            connectedRoads.Add(neighborRoad);
                        }
                    }
                }
            }
        }

        return connectedRoads;
    }
}
