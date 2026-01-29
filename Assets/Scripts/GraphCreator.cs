using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
    public static GraphCreator Instance;

    public GameObject prefabTile;
    [SerializeField] private int TileCount = 20;

    public float startX;
    public float startZ;

    private Graph mGraph = new Graph();
    private GameObject[,] tiles;
    bool foundok = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mGraph.populateGrid(TileCount);
        var grid = mGraph.mGrid;
        int count = mGraph.mCount;

        startX = -count / 2f;
        startZ = -startX;

        tiles = new GameObject[count, count];
        GameObject tilesParent = new GameObject("Tiles");
        
        for (int row = 0; row < count; row++)
        {
            for (int col = 0; col < count; col++)
            {
                float x = startX + col;
                float z = startZ - row;

                GameObject tile = Instantiate(
                    prefabTile,
                    new Vector3(x, 0f, z),
                    prefabTile.transform.rotation,
                    tilesParent.transform
                );

                tiles[row, col] = tile;
                PintarTile(row, col);
            }
        }
    }

    public void CalcularCamino(Vector2Int start, Vector2Int goal, PlayerScript player)
    {
        ResetVisual();
        foundok = false;

        mGraph.Reset();
        mGraph.SetStart(start);
        mGraph.SetGoal(goal);

        while (!foundok)
            foundok = mGraph.UpdateStep(tiles);

        MarcarCamino();

        List<sCell> path = mGraph.GetOptimalPath();
        if (path.Count == 0) return;

        sCell last = path.Last();
        Vector3 destino = GridToWorld(last.row, last.col);
        player.SetTarget(destino);
    }

    void PintarTile(int row, int col)
    {
        var rend = tiles[row, col].GetComponent<Renderer>();

        switch (mGraph.mGrid[row][col])
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

        foreach (var c in mGraph.ListaAbierta)
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.blue;

        foreach (var c in mGraph.ListaCerrada)
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.yellow;
    }


    void ResetVisual()
    {
        for (int r = 0; r < TileCount; r++)
            for (int c = 0; c < TileCount; c++)
                tiles[r, c].GetComponent<Renderer>().material.color = Color.white;
    }

    void MarcarCamino()
    {
        var path = mGraph.GetOptimalPath();
        foreach (var c in path)
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.cyan;
    }

    public Vector2Int WorldToGrid(Vector3 world)
    {
        int col = Mathf.RoundToInt(world.x - startX);
        int row = Mathf.RoundToInt(startZ - world.z);
        return new Vector2Int(row, col);
    }

    public Vector3 GridToWorld(int row, int col)
    {
        float x = startX + col;
        float z = startZ - row;
        return new Vector3(x, 0.5f, z);
    }
}
