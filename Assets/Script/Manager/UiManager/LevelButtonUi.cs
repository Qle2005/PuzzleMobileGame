using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButtonUi : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI titleText;
    //[SerializeField] private Image iconImage;
    [SerializeField] private Button actionButton;

    private LevelDataSO levelData;

    // Hàm khởi tạo dữ liệu cho nút khi được sinh ra
    public void Setup(LevelDataSO data)
    {
        levelData = data;

        if (titleText != null)
            titleText.text = levelData.levelName;

        // Đăng ký sự kiện click
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(OnLevelButtonClicked);
    }

    private void OnLevelButtonClicked()
    {
        if (levelData != null && !string.IsNullOrEmpty(levelData.levelName))
        {
            GameManager.Instance.SetLeveldata(levelData);
            GameManager.Instance.ButtonInvokeGameState(GameState.OnInGame);
            Debug.Log($"Đang chuyển tới Scene: {levelData.levelName} (Level: {levelData.levelIndex})");
        }
        else
        {
            Debug.LogError("Tên Scene chưa được cấu hình trong LevelDataSO!");
        }
    }
}
