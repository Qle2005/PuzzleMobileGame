using UnityEngine;

public class PlacementManager : MonoBehaviour
{
   public static PlacementManager Instance;

    [Header("Danh sách 3 điểm đặt cố định trong scene")]
    [SerializeField] private InitDraggableObjectFromUI[] spawnLocations; // Gán 3 object vào đây qua Inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Tìm điểm trống trong 3 điểm để spawn, nếu hết chỗ thì báo Log
    /// </summary>
    public void TrySpawnAndDrag(DraggableObject draggablePrefab, IGridService grid)
    {
        if (spawnLocations == null || spawnLocations.Length == 0) return;

        // Duyệt qua lần lượt các điểm đặt
        foreach (var spawner in spawnLocations)
        {
            if (spawner != null && !spawner.IsOccupied())
            {
                // Tìm thấy điểm trống đầu tiên -> Spawn vào đây và thoát hàm
                spawner.SpawnAndDragObject(draggablePrefab, grid);
                return;
            }
        }

        // Nếu chạy hết vòng lặp mà không return nghĩa là cả 3 điểm đều đang có vật thể
        Debug.LogWarning("Tất cả 3 điểm đặt (PlaceDraggableObject) đều đã đầy!");
    }
}
