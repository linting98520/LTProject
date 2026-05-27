using System;
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

    public Material MaterialNormal;
    public Material MaterialHover;
    public Material MaterialCantBuild;

    // 內部狀態
    private Cell[,] cells;
    public Cell HoveredCell { get; private set; }

    public void GenerateBoard()
    {
        ClearCell();
        CreateCells();
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
                Cell cell = CreateCell(x, z, pos, (x * Rows) + z);
                cells[x, z] = cell;
            }
        }
    }

    public Cell GetCell(int id)
    {
        int col = id / Rows;
        int row = id % Rows;
        return cells[col, row];
    }

    private Cell CreateCell(int x, int z, Vector3 position, int id)
    {
        GameObject go = Instantiate(CellObj, Root.transform);
        go.name = $"Cell_{x}_{z}";
        go.transform.SetParent(Root.transform);
        go.transform.localPosition = position;
        go.transform.localScale = new Vector3(CellSize, CellHeight, CellSize);

        Cell cell = go.GetComponent<Cell>();
        cell.Initialize(x, z, id);

        return cell;
    }

    public void EnterCell(Cell cell)
    {
        if (HoveredCell != null && HoveredCell != cell)
        {
            HoveredCell.SetMaterial(MaterialNormal);
        }

        cell.SetMaterial(cell.IsBuild ? MaterialCantBuild : MaterialHover);
        HoveredCell = cell;
    }

    public void ClearCell()
    {
        if (HoveredCell != null)
        {
            HoveredCell.SetMaterial(MaterialNormal);
            HoveredCell = null;
        }
    }
}