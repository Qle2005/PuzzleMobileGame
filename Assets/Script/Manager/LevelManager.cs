using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    private LevelDataSO currentLevelData;
    private IGridService gridService;
    private readonly List<OnMapObject> spawnedObjects = new List<OnMapObject>();

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

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResolveGridService();
        InitializeLevel();
    }

    public void LoadLevel(LevelDataSO newLevelData, string gameplaySceneName = "")
    {
        currentLevelData = newLevelData;

        bool shouldLoadScene = !string.IsNullOrEmpty(gameplaySceneName)
            && SceneManager.GetActiveScene().name != gameplaySceneName;

        if (shouldLoadScene)
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            ResolveGridService();
            InitializeLevel();
        }
    }

    public void RestartCurrentLevel()
    {
        if (currentLevelData != null)
        {
            LoadLevel(currentLevelData);
        }
    }

    private void InitializeLevel()
    {
        if (!CanInitializeLevel())
        {
            return;
        }

        ClearCurrentMap();
        SpawnMapObjects();
        SpawnStartEndpoints();
        InitializeInGameUi();
    }

    private void ResolveGridService()
    {
        gridService = FindFirstObjectByType<GridManager>();
    }

    private bool CanInitializeLevel()
    {
        if (gridService == null)
        {
            Debug.LogWarning("LevelManager: Không tìm thấy GridManager trong scene!");
            return false;
        }

        if (currentLevelData == null)
        {
            Debug.LogWarning("LevelManager: currentLevelData đang trống!");
            return false;
        }

        return true;
    }

    private void SpawnMapObjects()
    {
        foreach (var placement in currentLevelData.objectPlacements)
        {
            SpawnMapObject(placement);
        }
    }

    private void SpawnMapObject(LevelDataSO.MapObjectPlacement placement)
    {
        if (placement.mapObject == null)
        {
            return;
        }

        Vector3 worldPosition = gridService.CellToWorldPos(placement.originPos);
        OnMapObject mapObject = Instantiate(placement.mapObject, worldPosition, Quaternion.identity);
        mapObject.Init(gridService, placement.originPos);
        spawnedObjects.Add(mapObject);
    }

    private void SpawnStartEndpoints()
    {
        foreach (var point in currentLevelData.startEndpoints)
        {
            SpawnStartEndpoint(point);
        }
    }

    private void SpawnStartEndpoint(LevelDataSO.StartEndpoint points)
    {
        if (points.startPoint.mapObject == null || points.endPoint.mapObject == null)
        {
            return;
        }

        Vector3 startPosition = gridService.CellToWorldPos(points.startPoint.originPos);
        Vector3 endPosition = gridService.CellToWorldPos(points.endPoint.originPos);
        OnMapRoad startObject = (OnMapRoad)Instantiate(
            points.startPoint.mapObject,
            startPosition,
            Quaternion.identity);
        OnMapRoad endObject = (OnMapRoad)Instantiate(
            points.endPoint.mapObject,
            endPosition,
            Quaternion.identity);

        startObject.Init(gridService, points.startPoint.originPos);
        endObject.Init(gridService, points.endPoint.originPos);
        spawnedObjects.Add(startObject);
        spawnedObjects.Add(endObject);

        if (startObject is SpawnPoint startPoint)
        {
            startPoint.InitializeSpawnPoint(startObject, endObject, gridService);
        }

        if (endObject is SpawnPoint endPoint)
        {
            endPoint.SpawnUtlisObject();
        }
    }

    private void InitializeInGameUi()
    {
        if (UiManager.Instance == null)
        {
            Debug.LogWarning("LevelManager: Không tìm thấy UiManager.Instance!");
            return;
        }

        UiManager.Instance.InGameUiInitialized(currentLevelData, gridService);
    }

    private void ClearCurrentMap()
    {
        if (spawnedObjects.Count == 0) return;
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }
        }
        spawnedObjects.Clear();
    }
}