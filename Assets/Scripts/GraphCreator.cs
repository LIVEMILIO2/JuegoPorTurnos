using UnityEngine;
using System.Collections.Generic;

public class GraphCreator : MonoBehaviour
{
    public static GraphCreator Instance;

    public GameObject prefabTile;
    public int size = 10;

    public Graph graph = new();
    public GameObject[,] tiles;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        graph.PopulateGrid(size);
        tiles = new GameObject[size, size];

        float offset = size / 2f;

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                Vector3 pos = new(c - offset, 0, offset - r);
                tiles[r, c] = Instantiate(prefabTile, pos, Quaternion.identity);
                tiles[r, c].GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 pos)
    {
        int col = Mathf.RoundToInt(pos.x + size / 2f);
        int row = Mathf.RoundToInt(size / 2f - pos.z);
        return new Vector2Int(row, col);
    }

    public Vector3 GridToWorld(int r, int c)
    {
        return tiles[r, c].transform.position;
    }

    
    public void UpdateTileColors()
    {
        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                var rend = tiles[r, c].GetComponent<Renderer>();

                switch (graph.mGrid[r][c])
                {
                    case eCellType.start:
                        rend.material.color = Color.green;
                        break;
                    case eCellType.goal:
                        rend.material.color = Color.red;
                        break;
                    case eCellType.blocked:
                        rend.material.color = Color.black;
                        break;
                    default:
                        rend.material.color = Color.white;
                        break;
                }
            }
        }

        foreach (var c in graph.ListaAbierta)
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.blue;

        foreach (var c in graph.ListaCerrada)
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.yellow;
    }

    public void PaintPath(List<sCell> path)
    {
        foreach (var cell in path)
        {
            if (graph.mGrid[cell.row][cell.col] == eCellType.start) continue;
            if (graph.mGrid[cell.row][cell.col] == eCellType.goal) continue;

            tiles[cell.row, cell.col]
                .GetComponent<Renderer>()
                .material.color = Color.cyan;
        }
    }
}
