using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
    public static GraphCreator Instance;

    public GameObject prefabTile;

    [SerializeField]
    private int TileCount = 20;

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

        int count = mGraph.mCount;

        startX = -count / 2f;
        startZ = -startX;

        tiles = new GameObject[count, count];

        GameObject parent = new GameObject("Tiles");

        for (int row = 0; row < count; row++)
        {
            for (int col = 0; col < count; col++)
            {
                float x = startX + col;
                float z = startZ - row;

                GameObject tile =
                    Instantiate(
                        prefabTile,
                        new Vector3(x, 0, z),
                        Quaternion.identity,
                        parent.transform
                    );

                tiles[row, col] = tile;

                tile.GetComponent<Renderer>().material.color = Color.white;
            }
        }
    }


    public void CalcularCamino(
    Vector2Int start,
    Vector2Int goal,
    PlayerScript player
)
    {
        mGraph.Reset();
        ResetVisual();

        mGraph.SetStart(start);
        mGraph.SetGoal(goal);

        PintarTile(start.x, start.y);
        PintarTile(goal.x, goal.y);

        foundok = false;

        while (!foundok)
        {
            foundok = mGraph.UpdateStep(tiles);
        }

        MarcarCamino();

        List<sCell> pathGrid = mGraph.GetOptimalPath();

        if (pathGrid.Count < 2)
        {
            player.SetPath(new List<Vector3>());
            return;
        }

        List<Vector3> pathWorld = new List<Vector3>();

        int maxIndex = pathGrid.Count - 1;

        int pasos = Mathf.Min(
            player.playerMoveRange,
            maxIndex
        );

        for (int i = 1; i <= pasos; i++)
        {
            var c = pathGrid[i];

            pathWorld.Add(
                GridToWorld(c.row, c.col)
            );
        }

        player.SetPath(pathWorld);
    }
    public void CalcularCaminoEnemy(
        Vector2Int start,
        Vector2Int goal,
        EnemyScript enemy
    )
    {
        mGraph.Reset();

        ResetVisual();

        mGraph.SetStart(start);

        mGraph.SetGoal(goal);

        PintarTile(start.x, start.y);

        PintarTile(goal.x, goal.y);

        bool found = false;

        while (!found)
        {
            found = mGraph.UpdateStep(tiles);
        }

        var pathGrid = mGraph.GetOptimalPath();

        if (pathGrid.Count < 2)
        {
            enemy.SetPath(new List<Vector3>());
            return;
        }

        List<Vector3> pathWorld =
            new List<Vector3>();

        int maxIndex =
            pathGrid.Count - 2;

        int pasos =
            Mathf.Min(
                enemy.enemyMoveRange,
                maxIndex
            );

        for (int i = 1; i <= pasos; i++)
        {
            var c = pathGrid[i];

            pathWorld.Add(
                GridToWorld(
                    c.row,
                    c.col
                )
            );
        }

        enemy.SetPath(pathWorld);
    }



    public void ResetVisual()
    {
        for (int r = 0; r < TileCount; r++)
        {
            for (int c = 0; c < TileCount; c++)
            {
                tiles[r, c]
                .GetComponent<Renderer>()
                .material.color = Color.white;
            }
        }
    }

    void MarcarCamino()
    {
        var path =
            mGraph.GetOptimalPath();

        foreach (var c in path)
        {
            tiles[c.row, c.col]
            .GetComponent<Renderer>()
            .material.color = Color.cyan;
        }
    }

    void PintarTile(int row, int col)
    {
        var rend =
            tiles[row, col]
            .GetComponent<Renderer>();

        switch (mGraph.mGrid[row][col])
        {
            case eCellType.start:

                rend.material.color =
                    Color.green;

                break;

            case eCellType.goal:

                rend.material.color =
                    Color.red;

                break;
        }
    }


    public Vector2Int WorldToGrid(
        Vector3 world
    )
    {
        int col =
            Mathf.RoundToInt(
                world.x - startX
            );

        int row =
            Mathf.RoundToInt(
                startZ - world.z
            );

        return new Vector2Int(row, col);
    }

    public Vector3 GridToWorld(
        int row,
        int col
    )
    {
        float x = startX + col;

        float z = startZ - row;

        return new Vector3(
            x,
            0.5f,
            z
        );
    }
}