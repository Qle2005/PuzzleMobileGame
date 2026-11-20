using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public event Action<GameState> OnChangeGameState;
    public event Action OnGoState;

    [SerializeField] private List<LevelDataSO> levelDataList;

    private LevelDataSO currentSelectedLevel;
    public LevelDataSO CurrentSelectedLevel => currentSelectedLevel;


    private GameState currentGameState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        currentGameState = GameState.OnLevelMenu;
        ButtonInvokeGameState(currentGameState);
        InitializeLevelMenu();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void InitializeLevelMenu()
    {
        if (UiManager.Instance == null)
        {
            Debug.LogError("GameManager: UiManager.Instance chưa sẵn sàng!");
            return;
        }

        UiManager.Instance.LevelMenuInitialize(levelDataList);
    }

    private void HandleGameState(GameState newGameState)
    {
        switch (newGameState)
        {
            case GameState.OnGo:
                OnGoState?.Invoke();
                break;
            case GameState.OnInGame:
                StartLevel(currentSelectedLevel);
                break;
        }

        currentGameState = newGameState;
    }

    public void ButtonInvokeGameState(GameState newGameState)
    {
        currentGameState = newGameState;
        HandleGameState(currentGameState);
        OnChangeGameState?.Invoke(newGameState);
    }

    public void SetLeveldata(LevelDataSO newLevelDataSo)
    {
        currentSelectedLevel = newLevelDataSo;
    }

    /// <summary>
    /// Bắt đầu hoặc chuyển sang một Level mới
    /// </summary>
    public void StartLevel(LevelDataSO levelData, string gameplaySceneName = "")
    {
        currentSelectedLevel = levelData;

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.LoadLevel(currentSelectedLevel, gameplaySceneName);
        }
        else
        {
            Debug.LogError("GameManager: LevelManager.Instance chưa sẵn sàng!");
        }
    }

}