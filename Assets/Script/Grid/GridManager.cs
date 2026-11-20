using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GridManager : MonoBehaviour,IGridService
{
   
    //Scriptableobject
    [SerializeField] private int gridColumns = 10; // Số cột (chiều X)
    [SerializeField] private int gridRows = 10;    // Số hàng (chiều Z)
    //[SerializeField] private LevelData levelData; Đưa hai thằng trên về thành scriptableobject và thêm vào Prefab của map
    [SerializeField] private float snapHeight=0.5f;

    private Cell[,] cells;
    private Grid gridComponent;

    void Awake()
    {

        gridComponent = GetComponent<Grid>();

        if (gridComponent == null)
        {
            Debug.LogError("Thiếu component Grid trên GameObject này!");
            return;
        }

        InitializeGrid();
    }

    private void InitializeGrid()
    {
        cells = new Cell[gridColumns, gridRows];

        for (int x = 0; x < gridColumns; x++)
        {
            for (int z = 0; z < gridRows; z++)
            {
                // Khởi tạo vị trí Vector3Int khớp với mảng
                Vector3Int cellPosInt = new Vector3Int(x, 0, z);

                // Lấy vị trí World trực tiếp từ Grid Component
                Vector3 worldCellPos = gridComponent.CellToWorld(cellPosInt);

                Cell cell = new Cell(cellPosInt, worldCellPos);
                cells[x, z] = cell;
            }
        }
    }

    public Cell GetCell(Vector2Int vector2Int)
    {
        if (vector2Int.x < 0 || vector2Int.x >= cells.GetLength(0) ||
            vector2Int.y < 0 || vector2Int.y >= cells.GetLength(1))
        {
            return null;
        }
        return cells[vector2Int.x, vector2Int.y];
    }

    public Cell WorldPosToCell(Vector3 point)
    {
        Vector3Int cellInVector3Int = gridComponent.WorldToCell(point);

        Vector2Int cellInVector2Int = new Vector2Int(cellInVector3Int.x, cellInVector3Int.z);

        Cell cell = GetCell(cellInVector2Int);

        if (cell != null)
        {
            return cell;
        }
        else
        {
            return null;
        }
    }

    public Vector3 CellToWorldPos(Vector2Int originPos)
    {
        Vector3Int CellInVector3Int=new Vector3Int(originPos.x,0,originPos.y);
        Vector3 worldPos=gridComponent.CellToWorld(CellInVector3Int);
        return worldPos;
    }

    public List<Vector2Int> GetOccupiedCells(List<Vector2Int>occupiedCellList,Vector2Int origin, Quaternion rotation)
    {
        List<Vector2Int> occupied = new List<Vector2Int>();
        
       foreach (var cell in occupiedCellList)
       {
            // 1. Xoay vị trí tương đối của ô (cell) quanh gốc (0,0,0) trước
            Vector3 localOffset = new Vector3(cell.x, 0f, cell.y);
            Vector3 rotatedOffset = rotation * localOffset;
            
            // 2. Làm tròn tọa độ sau khi xoay
            int offsetX = Mathf.RoundToInt(rotatedOffset.x);
            int offsetZ = Mathf.RoundToInt(rotatedOffset.z);
            
            // 3. Cộng với vị trí gốc (origin) để ra vị trí thực tế trên Grid
            Vector2Int finalCell = origin + new Vector2Int(offsetX, offsetZ);
            
            occupied.Add(finalCell);
            Debug.Log($"Các cell đang bị chiếm chỗ là {GetCell(finalCell)}");
       }
        return occupied;
    }

    public bool CanPlaceObject(IOnMapObject draggable, Vector2Int originCellPos)
    {
        List<Vector2Int> occupiedCellPositions = GetOccupiedCells(draggable.OccupiedList, originCellPos, draggable.Rotation);

        foreach (var pos in occupiedCellPositions)
        {
            Cell cell = GetCell(pos);
            // Nếu vượt khỏi biên mảng OR cell đã có vật thể khác chiếm giữ
            if (cell == null || (cell.IsOccupied && cell.occupiedObject != draggable))
            {
                return false;
            }
        }
        return true;
    }

    public bool TrySnapToCell(Transform objTransform,out Vector2Int origin)
    {
        Vector3 currentPos = objTransform.position;
        Cell targetCell = WorldPosToCell(currentPos);

        if (targetCell != null)
        {
            Vector3 snapPosition = targetCell.CellInWorldPos;
            origin=new Vector2Int(targetCell.CellIn3int.x,targetCell.CellIn3int.z);
            snapPosition.y+=snapHeight;
            objTransform.position = snapPosition;
            return true;
        }
        else
        {
            origin=Vector2Int.zero;
            return false;
        }
    }

    public void PlaceObject(IOnMapObject draggable, Vector2Int originCellPos)
    {
        ClearObjectFromGrid(draggable);

        List<Vector2Int> occupiedCellPositions = GetOccupiedCells(draggable.OccupiedList, originCellPos, draggable.Rotation);
        foreach (var pos in occupiedCellPositions)
        {
            Cell cell = GetCell(pos);
            if (cell != null)
            {
                cell.occupiedObject = draggable;
                draggable.OccupiedCells.Add(cell); // Ghi nhận cell đang chiếm
            }
        }
        draggable.OriginCell = originCellPos;
    }

    public void ClearObjectFromGrid(IOnMapObject draggable)
    {
        if (draggable == null) return;

        // Duyệt trực tiếp danh sách cell đã lưu
        for (int i = 0; i < draggable.OccupiedCells.Count; i++)
        {
            Cell cell = draggable.OccupiedCells[i];
            if (cell != null && cell.occupiedObject == draggable)
            {
                cell.occupiedObject = null;
            }
        }
        draggable.OccupiedCells.Clear();
    }


    private void OnDrawGizmos()
    {
        Grid targetGrid = GetComponent<Grid>();
        if (targetGrid == null) return;

        Gizmos.color = Color.green;

        // Vẽ lưới dựa hoàn toàn vào CellToWorld của Grid Component để không bao giờ bị lệch
        for (int x = 0; x <= gridColumns; x++)
        {
            Vector3 start = targetGrid.CellToWorld(new Vector3Int(x, 0, 0))-new Vector3(0.5f,0,0.5f);
            Vector3 end = targetGrid.CellToWorld(new Vector3Int(x, 0, gridRows))-new Vector3(0.5f,0,0.5f);
            
            // Nhấc lên một chút so với mặt đất để dễ nhìn
            start.y += 0.01f;
            end.y += 0.01f;

            Gizmos.DrawLine(start, end);
        }

        for (int z = 0; z <= gridRows; z++)
        {
            Vector3 start = targetGrid.CellToWorld(new Vector3Int(0, 0, z))-new Vector3(0.5f,0,0.5f);
            Vector3 end = targetGrid.CellToWorld(new Vector3Int(gridColumns, 0, z))-new Vector3(0.5f,0,0.5f);

            start.y += 0.01f;
            end.y += 0.01f;

            Gizmos.DrawLine(start, end);
        }
        #if UNITY_EDITOR
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.normal.textColor = Color.yellow; // Màu chữ
        labelStyle.alignment = TextAnchor.MiddleCenter; // Căn giữa chữ
        labelStyle.fontSize = 10; // Kích thước chữ

        for (int x = 0; x < gridColumns; x++)
        {
            for (int z = 0; z < gridRows; z++)
            {
                // Vị trí tâm của ô (x, z)
                Vector3 cellCenter = targetGrid.GetCellCenterWorld(new Vector3Int(x, 0, z))-new Vector3(0.5f,0,0.5f);
                cellCenter.y += 0.02f; // Nhấc nhẹ lên để không bị khuất mặt đất

                // Chuỗi tọa độ
                string coordText = $"({x}, {z})";

                // Vẽ label ra Scene View
                Handles.Label(cellCenter, coordText, labelStyle);
            }
        }
        #endif
    }
}