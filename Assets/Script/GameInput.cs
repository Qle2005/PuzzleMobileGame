using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float dragThreshold = 10f; // Khoảng cách pixel tối thiểu để tính là hành động kéo

    public static GameInput Instance;

    private Plane dragPlane;
    private bool isDragging = false;
    private Vector2 screenPosition;
    private Vector2 pressScreenPosition;

    private DraggableObject potentialDraggable; // Vật thể vừa được bấm xuống (chờ xem có kéo không)
    private DraggableObject currentDraggable;    // Vật thể đang thực sự được kéo

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnTouchPosition(InputValue inputValue)
    {
        screenPosition = inputValue.Get<Vector2>();

        // 1. Nếu chưa chuyển sang trạng thái kéo, nhưng đang giữ chuột/tay trên một DraggableObject
        // Kiểm tra xem ngón tay có dịch chuyển vượt quá ngưỡng cho phép chưa
        if (!isDragging && potentialDraggable != null)
        {
            float distance = Vector2.Distance(screenPosition, pressScreenPosition);
            if (distance > dragThreshold)
            {
                StartActualDrag(potentialDraggable);
            }
        }

        // 2. Nếu đang trong trạng thái kéo thực sự
        if (isDragging && currentDraggable != null)
        {
            if (TryGetWorldPoint(screenPosition, out Vector3 worldPoint))
            {
                currentDraggable.HandleDragUpdate(worldPoint);
            }
        }
    }

    private void OnTouchPress(InputValue inputValue)
    {
        bool isPressed = inputValue.isPressed;

        if (isPressed)
        {
            HandlePressDown();
        }
        else
        {
            HandlePressUp();
        }
    }

    private void HandlePressDown()
    {
        pressScreenPosition = screenPosition;
        potentialDraggable = null;

        Ray ray = mainCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("Chạm vào: " + hit.collider.gameObject.name);

            // A. Nếu chạm vào ô đường cố định (OnMapRoad) -> Xoay luôn vì ô đường không cần kéo thả
            if (hit.collider.TryGetComponent<IRotable>(out IRotable rotatable) && hit.collider.TryGetComponent<OnMapRoad>(out _))
            {
                rotatable.Rotate();
                return;
            }

            // B. Nếu chạm vào DraggableObject -> TẠM THỜI CHƯA LÀM GÌ CẢ (Chờ xem người dùng muốn kéo hay muốn bấm)
            if (hit.collider.TryGetComponent<DraggableObject>(out DraggableObject draggable))
            {
                potentialDraggable = draggable;
            }
        }
    }

    private void StartActualDrag(DraggableObject draggable)
    {
        isDragging = true;
        currentDraggable = draggable;
        potentialDraggable = null; // Xóa tiềm năng bấm vì đã chuyển sang kéo

        dragPlane = new Plane(Vector3.up, draggable.transform.position);

        if (TryGetWorldPoint(screenPosition, out Vector3 worldPoint))
        {
            currentDraggable.HandleDragStart(worldPoint);
        }
    }

    private void HandlePressUp()
    {
        // Nếu đang trong quá trình kéo và thả tay ra -> Kết thúc kéo
        if (isDragging && currentDraggable != null)
        {
            if (TryGetWorldPoint(screenPosition, out Vector3 worldPoint))
            {
                currentDraggable.HandleDragEnd(worldPoint);
            }
            currentDraggable = null;
            isDragging = false;
        }
        // Nếu KHÔNG kéo (chỉ chạm xuống rồi nhấc lên nhanh) VÀ có chạm vào DraggableObject -> Tính là Click/Tap để xoay
        else if (potentialDraggable != null)
        {
            // Gọi hàm xoay (bên trong DraggableObject sẽ tự kiểm tra xem đã đặt thành công hay chưa)
            potentialDraggable.Rotate();
            potentialDraggable = null;
        }
    }

    private bool TryGetWorldPoint(
        Vector2 touchScreenPoint,
        out Vector3 worldPoint)
    {
        worldPoint = Vector3.zero;
        Ray ray = mainCamera.ScreenPointToRay(touchScreenPoint);

        if (dragPlane.Raycast(ray, out float enter))
        {
            worldPoint = ray.GetPoint(enter);
            return true;
        }

        return false;
    }
}