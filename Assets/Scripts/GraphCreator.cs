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

    private HashSet<(int, int)> tilesEnRango = new HashSet<(int, int)>();

    private static readonly Color colorPlayer = new Color(0.2f, 0.6f, 1f, 1f);
    private static readonly Color colorEnemy = new Color(1f, 0.25f, 0.25f, 1f);

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

    // ─── Tiles de unidades ───────────────────────────────────────────────────

    public void PintarTilesUnidades()
    {
        foreach (var p in GameManager.Instance.players)
        {
            if (p == null) continue;
            Vector2Int grid = WorldToGrid(p.transform.position);
            if (grid.x >= 0 && grid.x < TileCount && grid.y >= 0 && grid.y < TileCount)
                tiles[grid.x, grid.y].GetComponent<Renderer>().material.color = colorPlayer;
        }

        foreach (var e in GameManager.Instance.enemies)
        {
            if (e == null) continue;
            Vector2Int grid = WorldToGrid(e.transform.position);
            if (grid.x >= 0 && grid.x < TileCount && grid.y >= 0 && grid.y < TileCount)
                tiles[grid.x, grid.y].GetComponent<Renderer>().material.color = colorEnemy;
        }
    }

    /// <summary>
    /// Actualiza en tiempo real el tile de una unidad mientras se mueve.
    /// Llámalo cada frame desde Mover() en PlayerScript y EnemyScript.
    /// </summary>
    public void ActualizarTileUnidad(Vector3 posAnterior, Vector3 posActual, bool esPlayer)
    {
        Vector2Int gridAnterior = WorldToGrid(posAnterior);
        Vector2Int gridActual = WorldToGrid(posActual);

        if (gridAnterior == gridActual) return;

        // Limpiar tile anterior solo si no hay otra unidad encima
        bool hayOtraUnidad = false;
        foreach (var p in GameManager.Instance.players)
            if (p != null && WorldToGrid(p.transform.position) == gridAnterior) { hayOtraUnidad = true; break; }
        if (!hayOtraUnidad)
            foreach (var e in GameManager.Instance.enemies)
                if (e != null && WorldToGrid(e.transform.position) == gridAnterior) { hayOtraUnidad = true; break; }

        if (!hayOtraUnidad &&
            gridAnterior.x >= 0 && gridAnterior.x < TileCount &&
            gridAnterior.y >= 0 && gridAnterior.y < TileCount)
            tiles[gridAnterior.x, gridAnterior.y].GetComponent<Renderer>().material.color = Color.white;

        // Pintar tile actual
        if (gridActual.x >= 0 && gridActual.x < TileCount &&
            gridActual.y >= 0 && gridActual.y < TileCount)
            tiles[gridActual.x, gridActual.y].GetComponent<Renderer>().material.color =
                esPlayer ? colorPlayer : colorEnemy;
    }

    // ─── Rango de movimiento ─────────────────────────────────────────────────

    public void MostrarRangoMovimiento(Vector2Int origen, int rango)
    {
        ResetVisual();
        tilesEnRango.Clear();

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

        for (int r = 0; r < TileCount; r++)
            for (int c = 0; c < TileCount; c++)
                tiles[r, c].GetComponent<Renderer>().material.color =
                    tilesEnRango.Contains((r, c))
                        ? new Color(0.3f, 1f, 0.3f, 1f)
                        : new Color(0.8f, 0.8f, 0.8f, 1f);

        tiles[origen.x, origen.y].GetComponent<Renderer>().material.color = Color.cyan;

        foreach (var e in GameManager.Instance.enemies)
        {
            if (e == null) continue;
            Vector2Int grid = WorldToGrid(e.transform.position);
            if (grid.x >= 0 && grid.x < TileCount && grid.y >= 0 && grid.y < TileCount)
                tiles[grid.x, grid.y].GetComponent<Renderer>().material.color = colorEnemy;
        }
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
            ResetVisual();
            return;
        }

        int pasos = Mathf.Min(player.playerMoveRange, pathGrid.Count - 1);
        List<Vector3> pathWorld = new List<Vector3>();
        for (int i = 1; i <= pasos; i++)
            pathWorld.Add(GridToWorld(pathGrid[i].row, pathGrid[i].col));

        player.SetPath(pathWorld);
        ResetVisual(); // limpia el camino pintado, deja solo tiles de unidades
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
            ResetVisual();
            return;
        }

        int pasos = Mathf.Min(enemy.enemyMoveRange, pathGrid.Count - 2);
        List<Vector3> pathWorld = new List<Vector3>();
        for (int i = 1; i <= pasos; i++)
            pathWorld.Add(GridToWorld(pathGrid[i].row, pathGrid[i].col));

        enemy.SetPath(pathWorld);
        ResetVisual(); // limpia el camino pintado, deja solo tiles de unidades
    }

    // ─── Visual ──────────────────────────────────────────────────────────────

    public void ResetVisual()
    {
        tilesEnRango.Clear();
        for (int r = 0; r < TileCount; r++)
            for (int c = 0; c < TileCount; c++)
                tiles[r, c].GetComponent<Renderer>().material.color = Color.white;

        PintarTilesUnidades();
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