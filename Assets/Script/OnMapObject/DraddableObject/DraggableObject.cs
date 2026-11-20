using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.PlayerLoop;
[RequireComponent(typeof(Collider))]
public class DraggableObject : MonoBehaviour,IDraggableObject, IRotable
{
    [Header("Mảng các vị trí chiếm chỗ")]
    [Tooltip("Tọa độ (0,0) bắt đầu từ pivot của vật")]
    [SerializeField] List<Vector2Int> occupiedList;

    [Header("Trạng thái")]
    public bool isPlaced = false;

    [Header("Giá của vật thể này")]
    [SerializeField] int price;

    [Header("Danh sách điểm mốc xe chạy qua")]
    [SerializeField] List<Transform> forwardWaypoints;
    [SerializeField] List<Transform> backwardWaypoints;

    [Header("Cổng kết nối gốc (khi Rotation = 0)")]
    [SerializeField] bool baseConnectNorth;
    [SerializeField] bool baseConnectSouth;
    [SerializeField] bool baseConnectEast;
    [SerializeField] bool baseConnectWest;

    public event Action<DraggableObjectState> OnStateChanged;
    public Vector2Int OriginCell { get; set; }
    public List<Vector2Int> OccupiedList=>occupiedList;
    public Quaternion Rotation => this.transform.rotation;
    public List<Cell> OccupiedCells => placedCells;
    public int Price=>price;
    public List<Transform> ForwardWaypoints => forwardWaypoints;
    public List<Transform> BackwardWaypoints => backwardWaypoints;

    
    
    private List<Cell> placedCells { get; } = new List<Cell>();


    private Vector3 originalPosition;
    private Vector2Int originalOriginCell;
    private bool isSelected = false;
    private DraggableObjectState currentObjectState=DraggableObjectState.None;
    private IGridService grid;
    private InitDraggableObjectFromUI homeSpawner; // Lưu lại điểm xuất phát của nó
    
    private void Start()
    {
        originalPosition = transform.position;
        ChangeObjectState(DraggableObjectState.Origin);

        if (grid != null)
        {
            Cell startCell = grid.WorldPosToCell(transform.position);
            if (startCell != null)
            {
                originalOriginCell = new Vector2Int(startCell.CellIn3int.x, startCell.CellIn3int.z);
                grid.PlaceObject(this, originalOriginCell);
            }
        }
    }

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

    private void ChangeObjectState(DraggableObjectState newObjectState)
    {
        if(currentObjectState==newObjectState) return;
        currentObjectState=newObjectState;
        Debug.Log(this.gameObject.name +" đang ở trạng thái" + currentObjectState);
        OnStateChanged?.Invoke(newObjectState);
    }

    public void Init(IGridService gridService, InitDraggableObjectFromUI spawner = null)
    {
        this.grid = gridService;
        this.homeSpawner = spawner;
    }


    public void HandleDragStart(Vector3 worldPoint)
    {
        isSelected = true;
        isPlaced = false;
        //originalPosition = transform.position;
    

        // Tạm thời gỡ khỏi Grid Data khi đang kéo
        if (grid != null)
        {
            grid.ClearObjectFromGrid(this);
        }

        Debug.Log($"Chạm vào vật thể: {gameObject.name}");
    }

    public void HandleDragUpdate(Vector3 worldPoint)
    {
        if (!isSelected) return;

        // Cập nhật vị trí kéo theo tay/chuột
        transform.position = new Vector3(worldPoint.x, originalPosition.y, worldPoint.z);

        if (grid != null)
        {
            if (grid.TrySnapToCell(transform,out Vector2Int originPos))
            {
                // Kiểm tra xem tất cả ô mà nó chiếm có đặt được không
                bool canPlace = grid.CanPlaceObject(this, originPos);
                
                // Kết hợp với Physical BoxCast va chạm phía dưới 
                if (canPlace)
                {
                    ChangeObjectState(DraggableObjectState.CanPlace);
                }
                else
                {
                    ChangeObjectState(DraggableObjectState.CannotPlace);
                }
            }
            else
            {
                ChangeObjectState(DraggableObjectState.CannotPlace);
            }
        }
    }

    public void HandleDragEnd(Vector3 worldPoint)
    {
       if (!isSelected) return;
        isSelected = false;

        if (grid != null)
        {
            Cell currentCell = grid.WorldPosToCell(transform.position);

            if (currentCell != null && currentObjectState == DraggableObjectState.CanPlace)
            {
                Vector2Int originPos = new Vector2Int(currentCell.CellIn3int.x, currentCell.CellIn3int.z);

                // Đặt thành công vào vị trí mới
                isPlaced = true;
                grid.PlaceObject(this, originPos);
                originalOriginCell = originPos;
                originalPosition = transform.position;
                
                ChangeObjectState(DraggableObjectState.Origin);

                if (homeSpawner != null)
                {
                    homeSpawner.ClearSlot();
                    homeSpawner = null; // Ngắt kết nối sau khi đã giải phóng
                }
            }
            else
            {
                // Thất bại: Trả về vị trí cũ
                transform.position = originalPosition;
                grid.PlaceObject(this, originalOriginCell);
                ChangeObjectState(DraggableObjectState.Origin);
            }
        }
    }

    public void OnDrawGizmosSelected()
    {
            // Đánh dấu Pivot của GameObject
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.05f);

        Gizmos.color = Color.red;
        Vector3 origin = transform.position;
        Vector3 halfExtents = new Vector3(0.45f, 0.05f, 0.45f);
        
        // Vẽ vị trí bắt đầu
        Gizmos.DrawWireCube(origin, halfExtents * 2);
        // Vẽ vị trí kết thúc khi quét xuống 1m
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(origin + Vector3.down * 1f, halfExtents * 2);
    }

    public void Rotate()
    {
        if (!isPlaced) return;
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
