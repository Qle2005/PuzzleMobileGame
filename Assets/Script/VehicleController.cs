using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float reachThreshold;
    [SerializeField] bool canGo = false;

    private int currentIndex=0;
    private List<Transform> Waypoints  = new List<Transform>();
    private OnMapRoad startRoad;
    private OnMapRoad endRoad;
    private IGridService gridService;

    public void Initialize(OnMapRoad _startPoint, OnMapRoad _endPoint, IGridService _gridManager)
    {
        startRoad=_startPoint;
        endRoad=_endPoint;
        gridService = _gridManager;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoState += TriggerVehicleDrive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DriverHandller();
       
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGoState -= TriggerVehicleDrive;
        }
    }

    private void TriggerVehicleDrive()
    {
        Waypoints = RoadPathFinder.FindPath(startRoad, endRoad, gridService);
        canGo = true;
    }

    private void DriverHandller()
    {
        if (!canGo) return;
        // Nếu danh sách trống thì không làm gì cả
        if (Waypoints == null || Waypoints.Count == 0) return;  
        

        // Lấy mục tiêu hiện tại theo index
        Transform targetWaypoint = Waypoints[currentIndex];

        // 1. Dịch chuyển dần từ vị trí hiện tại đến vị trí mục tiêu
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        //Xoay mặt vật thể hướng về điểm đang di chuyển
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;
        if (direction != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(direction);

        // 2. Kiểm tra xem đã đến gần điểm đó chưa
        if (Vector3.Distance(transform.position, targetWaypoint.position) <= reachThreshold)
        {
            // Chuyển sang điểm tiếp theo trong danh sách
            currentIndex++;

            // Nếu đã đi hết danh sách:
            if (currentIndex >= Waypoints.Count)
            {
                Debug.Log("Bạn đã đến đích");
                canGo=false;
                return;
            }
        }

    }
}
