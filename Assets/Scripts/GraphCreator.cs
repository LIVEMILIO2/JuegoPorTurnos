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

    // Tiles actualmente pintados como rango de movimiento
    private HashSet<(int, int)> tilesEnRango = new HashSet<(int, int)>();

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

                GameObject tile = Instantiate(
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

    // ─── Rango de movimiento ─────────────────────────────────────────────────

    /// <summary>
    /// Pinta los tiles alcanzables en verde y los demás en rojo.
    /// Usar BFS desde la posición del player hasta playerMoveRange pasos.
    /// </summary>
    public void MostrarRangoMovimiento(Vector2Int origen, int rango)
    {
        ResetVisual();
        tilesEnRango.Clear();

        // BFS para encontrar todos los tiles alcanzables
        Queue<(Vector2Int pos, int pasos)> cola = new Queue<(Vector2Int, int)>();
        HashSet<Vector2Int> visitados = new HashSet<Vector2Int>();

        cola.Enqueue((origen, 0));
        visitados.Add(origen);

        while (cola.Count > 0)
        {
            var (pos, pasos) = cola.Dequeue();
            tilesEnRango.Add((pos.x, pos.y));

            if (pasos >= rango) continue;

            Vector2Int[] vecinos = {
                new Vector2Int(pos.x - 1, pos.y),
                new Vector2Int(pos.x + 1, pos.y),
                new Vector2Int(pos.x, pos.y - 1),
                new Vector2Int(pos.x, pos.y + 1)
            };

            foreach (var v in vecinos)
            {
                if (v.x < 0 || v.y < 0 || v.x >= TileCount || v.y >= TileCount) continue;
                if (mGraph.mGrid[v.x][v.y] == eCellType.blocked) continue;
                if (visitados.Contains(v)) continue;

                visitados.Add(v);
                cola.Enqueue((v, pasos + 1));
            }
        }

        // Pintar tiles
        for (int r = 0; r < TileCount; r++)
        {
            for (int c = 0; c < TileCount; c++)
            {
                if (tilesEnRango.Contains((r, c)))
                    tiles[r, c].GetComponent<Renderer>().material.color = new Color(0.3f, 1f, 0.3f, 1f); // Verde claro
                else
                    tiles[r, c].GetComponent<Renderer>().material.color = new Color(1f, 0.3f, 0.3f, 1f); // Rojo claro
            }
        }

        // El tile del jugador en azul
        tiles[origen.x, origen.y].GetComponent<Renderer>().material.color = Color.cyan;
    }

    // ─── Caminos ─────────────────────────────────────────────────────────────

    public void CalcularCamino(Vector2Int start, Vector2Int goal, PlayerScript player)
    {
        mGraph.Reset();
        ResetVisual();

        mGraph.SetStart(start);
        mGraph.SetGoal(goal);
        PintarTile(start.x, start.y);
        PintarTile(goal.x, goal.y);

        foundok = false;
        while (!foundok)
            foundok = mGraph.UpdateStep(tiles);

        List<sCell> pathGrid = mGraph.GetOptimalPath();

        if (pathGrid.Count < 2)
        {
            player.SetPath(new List<Vector3>());
            return;
        }

        int pasos = Mathf.Min(player.playerMoveRange, pathGrid.Count - 1);
        List<Vector3> pathWorld = new List<Vector3>();

        for (int i = 1; i <= pasos; i++)
            pathWorld.Add(GridToWorld(pathGrid[i].row, pathGrid[i].col));

        player.SetPath(pathWorld);
    }

    public void CalcularCaminoEnemy(Vector2Int start, Vector2Int goal, EnemyScript enemy)
    {
        mGraph.Reset();
        ResetVisual();

        mGraph.SetStart(start);
        mGraph.SetGoal(goal);
        PintarTile(start.x, start.y);
        PintarTile(goal.x, goal.y);

        bool found = false;
        while (!found)
            found = mGraph.UpdateStep(tiles);

        var pathGrid = mGraph.GetOptimalPath();

        if (pathGrid.Count < 2)
        {
            enemy.SetPath(new List<Vector3>());
            return;
        }

        int pasos = Mathf.Min(enemy.enemyMoveRange, pathGrid.Count - 2);
        List<Vector3> pathWorld = new List<Vector3>();

        for (int i = 1; i <= pasos; i++)
            pathWorld.Add(GridToWorld(pathGrid[i].row, pathGrid[i].col));

        enemy.SetPath(pathWorld);
    }

    // ─── Visual ──────────────────────────────────────────────────────────────

    public void ResetVisual()
    {
        tilesEnRango.Clear();
        for (int r = 0; r < TileCount; r++)
            for (int c = 0; c < TileCount; c++)
                tiles[r, c].GetComponent<Renderer>().material.color = Color.white;
    }

    void MarcarCamino()
    {
        foreach (var c in mGraph.GetOptimalPath())
            tiles[c.row, c.col].GetComponent<Renderer>().material.color = Color.cyan;
    }

    void PintarTile(int row, int col)
    {
        var rend = tiles[row, col].GetComponent<Renderer>();
        switch (mGraph.mGrid[row][col])
        {
            case eCellType.start: rend.material.color = Color.green; break;
            case eCellType.goal: rend.material.color = Color.red; break;
        }
    }

    // ─── Conversión ──────────────────────────────────────────────────────────

    public Vector2Int WorldToGrid(Vector3 world)
    {
        int col = Mathf.RoundToInt(world.x - startX);
        int row = Mathf.RoundToInt(startZ - world.z);
        return new Vector2Int(row, col);
    }

    public Vector3 GridToWorld(int row, int col)
    {
        return new Vector3(startX + col, 0.5f, startZ - row);
    }
}