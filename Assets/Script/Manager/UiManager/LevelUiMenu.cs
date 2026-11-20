using System.Collections.Generic;
using UnityEngine;

public class LevelUiMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform contentContainer;    
    [SerializeField] private GameObject levelButtonPrefab;  


    private void Start()
    {
        
    }

    public void GenerateLevelButtons(List<LevelDataSO> dataSource)
    {
        // 1. Dọn dẹp các button cũ (nếu có gọi lại hàm refresh)
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

        // 2. Khởi tạo button mới cho từng LevelDataSO trong danh sách
        foreach (var levelData in dataSource)
        {
            if (levelData == null) continue;

            GameObject buttonObj = Instantiate(levelButtonPrefab, contentContainer);
            LevelButtonUi buttonUI = buttonObj.GetComponent<LevelButtonUi>();

            if (buttonUI != null)
            {
                buttonUI.Setup(levelData);
            }
            else
            {
                Debug.LogError("Prefab Button thiếu component LevelButtonUI!");
            }
        }
    }
}
