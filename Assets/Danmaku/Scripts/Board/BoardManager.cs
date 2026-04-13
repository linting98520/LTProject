using UnityEngine;
using UnityEngine.Events;

public class BoardManager : MonoBehaviour
{
    [Header("棋盤尺寸設定")]
    [Range(2, 20)]
    public int Columns = 8;
    [Range(2, 20)]
    public int Rows = 8;
    [Range(0.5f, 5f)]
    public float CellSize = 1f;
    [Range(0f, 0.5f)]
    public float CellGap = 0.05f;

    [Header("格子外觀")]
    public float CellHeight = 0.1f;

    public GameObject CellObj;
    public GameObject Root;

    // 內部狀態
    private Cell[,] cells;
    private Cell hoveredCell;
    private Cell selectedCell;

    private void Start()
    {
        GenerateBoard();
    }

    public void GenerateBoard()
    {
        ClearBoard();
        CreateCells();
    }

    private void ClearBoard()
    {
        cells = null;
        hoveredCell = null;
        selectedCell = null;
    }

    private void CreateCells()
    {
        cells = new Cell[Columns, Rows];

        float totalWidth = Columns * (CellSize + CellGap) - CellGap;
        float totalDepth = Rows * (CellSize + CellGap) - CellGap;
        float startX = -totalWidth / 2f + CellSize / 2f;
        float startZ = -totalDepth / 2f + CellSize / 2f;

        for (int x = 0; x < Columns; x++)
        {
            for (int z = 0; z < Rows; z++)
            {
                Vector3 pos = new Vector3(startX + x * (CellSize + CellGap), 0f, startZ + z * (CellSize + CellGap));
                Cell cell = CreateCell(x, z, pos);
                cells[x, z] = cell;
            }
        }
    }

    private Cell CreateCell(int x, int z, Vector3 position)
    {
        GameObject go = Instantiate(CellObj, Root.transform);
        go.name = $"Cell_{x}_{z}";
        go.transform.SetParent(Root.transform);
        go.transform.localPosition = position;
        go.transform.localScale = new Vector3(CellSize, CellHeight, CellSize);

        Cell cell = go.AddComponent<Cell>();
        cell.Initialize(x, z);

        return cell;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying && Root != null)
            GenerateBoard();
    }
#endif
}

[System.Serializable]
public struct CellClickData
{
    public int Column;
    public int Row;
    public Vector3 WorldPosition;

    public CellClickData(int col, int row, Vector3 pos)
    {
        Column = col; Row = row; WorldPosition = pos;
    }

    public override string ToString() => $"Cell({Column},{Row}) @ {WorldPosition}";
}