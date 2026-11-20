using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUiManage : MonoBehaviour
{
    [Header("Cấu hình nút bấm trong level")]
    [SerializeField] private Button goButton;
    [SerializeField] private Button restartButton;
    [Header("Cấu hình slot trong scroll bar")]
    [SerializeField] private UiSlotWorldArchor slotPrefab;
    [SerializeField] private Transform slotContainer; // Transform cha (ví dụ Panel hoặc Layout Group) chứa các slot
    [SerializeField] private TextMeshProUGUI budgetUi;

    private void Start()
    {
        RegisterButton();
    }

    private void RegisterButton()
    {
        goButton.onClick.AddListener(() =>
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ButtonInvokeGameState(GameState.OnGo);
            }
            else Debug.LogError("UiManager chưa khởi tạo hoàn tất trong InGameUiManage");
        });
    }

    public void SetupSlots(List<DraggableObject> draggableObjects, IGridService grid)
    {
        if (slotPrefab == null || slotContainer == null)
        {
            Debug.LogWarning("Chưa gán slotPrefab hoặc slotContainer trong UiManager!");
            return;
        }

        // Xóa các slot cũ nếu có
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // Tạo slot mới và gán dữ liệu từng phần tử
        foreach (var draggable in draggableObjects)
        {
            if (draggable != null)
            {
                UiSlotWorldArchor newSlot = Instantiate(slotPrefab, slotContainer);
                newSlot.Init(draggable, grid);
            }
        }
    }

    /// <summary>
    /// Nhận Budget từ LevelData của LevelManager truyền cho BudgetManager và gắn cho TextMesh
    /// </summary>
    public void ShowBudgetUi(int levelDataBudget)
    {
        if (BudgetManager.Instance != null)
        {
            int currentBudget = BudgetManager.Instance.SetUpBudget(levelDataBudget);
            Debug.Log($"Budget hiện tại là : {currentBudget}");
            UpdateDisplay(currentBudget);

        }
        else
        {
            Debug.LogWarning("BudgetManager.Instance đang bị null!");
        }
    }

    public void UpdateDisplay(int newBudget)
    {
        if (budgetUi != null)
        {
            budgetUi.text = newBudget.ToString("D4");
        }
    }
   
}
