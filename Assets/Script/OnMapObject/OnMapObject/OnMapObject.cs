using System.Collections.Generic;
using UnityEngine;

public class OnMapObject : MonoBehaviour,IOnMapObject
{
    [SerializeField] List<Vector2Int> occupiedList;

   

    public Vector2Int OriginCell { get; set; }

    public List<Vector2Int> OccupiedList => occupiedList;

    public Quaternion Rotation => this.transform.rotation;

    public List<Cell> OccupiedCells => placedCells;
   

    private IGridService grid;
    private List<Cell> placedCells { get; } = new List<Cell>();



    public void Init(IGridService gridService, Vector2Int originCell)
    {
        grid = gridService;
        OriginCell = originCell;

        if (grid != null)
        {
            // Tự động đăng ký vị trí của mình với GridManager
            grid.PlaceObject(this, OriginCell);
        }
        else
        {
            Debug.LogWarning($"OnMapObject {gameObject.name} chưa nhận được GridService để đăng ký!", this);
        }
    }

   
}
