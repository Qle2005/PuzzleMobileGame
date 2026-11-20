using UnityEngine;

public class InitDraggableObjectFromUI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform spawnPoint; 

    // Biến lưu giữ reference đến vật thể hiện đang nằm trên điểm này
    private DraggableObject currentSpawnedObject;

    private void Awake()
    {
        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }
    }

    /// <summary>
    /// Kiểm tra xem điểm này đã có vật thể chưa (và vật thể đó còn tồn tại không)
    /// </summary>
    public bool IsOccupied()
    {
        // Nếu object bị destroy hoặc null thì tính là trống
        if (currentSpawnedObject == null) return false;
        
        return true;
    }

    /// <summary>
    /// Thực hiện Spawn vật thể tại điểm này
    /// </summary>
    public void SpawnAndDragObject(DraggableObject draggablePrefab, IGridService grid)
    {
        if (draggablePrefab == null) return;

        // Instantiate tại vị trí của điểm này
        currentSpawnedObject = Instantiate(draggablePrefab, spawnPoint.position, spawnPoint.rotation);

        // Khởi tạo logic của object
        currentSpawnedObject.Init(grid,this);

    }

    /// <summary>
    /// Hàm này được gọi từ DraggableObject khi nó đã thả thành công xuống map grid
    /// </summary>
    public void ClearSlot()
    {
        currentSpawnedObject = null;
    }
}
