using System.Collections.Generic;
using UnityEngine;

public class UiManager : MonoBehaviour
{
    public static UiManager Instance { get; private set; }

    [Header("Cấu hình các game object UI")]
    [SerializeField] private GameObject inGameUiObject;
    [SerializeField] private GameObject levelUiObject;

    private InGameUiManage inGameUiManage;
    private LevelUiMenu levelUiManage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        GameManager.Instance.OnChangeGameState += HandleUiGameState;
        CacheUiReferences();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    private void CacheUiReferences()
    {
        inGameUiManage = GetComponentFromObject<InGameUiManage>(
            inGameUiObject,
            "inGameUiObject chưa được gán hoặc thiếu thành phần InGameUiManage");
        levelUiManage = GetComponentFromObject<LevelUiMenu>(
            levelUiObject,
            "levelUiObject chưa được gán hoặc thiếu thành phần LevelUiMenu");
    }

    private T GetComponentFromObject<T>(GameObject target, string errorMessage)
        where T : Component
    {
        if (target != null && target.TryGetComponent(out T component))
        {
            return component;
        }

        Debug.LogError($"UiManager: {errorMessage}");
        return null;
    }

    private void HandleUiGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.OnLevelMenu:
                SetActiveUi(levelUiObject);
                break;
            case GameState.OnInGame:
                SetActiveUi(inGameUiObject);
                break;
            default:
                Debug.LogWarning($"UiManager: Không có UI được định nghĩa cho GameState {newGameState}");
                break;
        }
    }

    private void SetActiveUi(GameObject target)
    {
        if (inGameUiObject != null)
            inGameUiObject.SetActive(target == inGameUiObject);
        if (levelUiObject != null)
            levelUiObject.SetActive(target == levelUiObject);
    }

    public void LevelMenuInitialize(List<LevelDataSO> dataSource)
    {
        if (levelUiManage == null)
        {
            Debug.LogError("UiManager: levelUiManage chưa được khởi tạo");
            return;
        }

        levelUiManage.GenerateLevelButtons(dataSource);
    }

    public void InGameUiInitialized(LevelDataSO levelData, IGridService gridService)
    {
        if (levelData == null)
        {
            Debug.LogWarning("UiManager: Không thể khởi tạo UI khi levelData là null");
            return;
        }

        inGameUiManage?.SetupSlots(levelData.draggableObjects, gridService);
        inGameUiManage?.ShowBudgetUi(levelData.Budget);
    }

    public void UpdateBudgetDisplay(int newBudget)
    {
        inGameUiManage?.UpdateDisplay(newBudget);
    }
}