using UnityEngine;
using UnityEngine.EventSystems;

public class UiSlotWorldArchor : MonoBehaviour, IPointerDownHandler
{
    private DraggableObject draggablePrefab;
    private IGridService grid;

    public void Init(DraggableObject prefab, IGridService gridService)
    {
        draggablePrefab = prefab;
        grid=gridService;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (draggablePrefab == null) return;

        if (BudgetManager.Instance != null)
        {
            int draggableObjectPrice=-draggablePrefab.Price;
            int updateBudget=BudgetManager.Instance.CalculateBudget(draggableObjectPrice);
            if (UiManager.Instance != null)
            {
                UiManager.Instance.UpdateBudgetDisplay(updateBudget);
            }
        }
        
        // Yêu cầu PlacementManager tìm điểm trống trong 3 điểm để cấp phát
        if (PlacementManager.Instance != null)
        {
            PlacementManager.Instance.TrySpawnAndDrag(draggablePrefab, grid);
        }
    }
}