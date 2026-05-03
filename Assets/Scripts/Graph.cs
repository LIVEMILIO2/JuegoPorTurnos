using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum eCellType
{
    empty,
    blocked,
    start,
    goal
}

public class sCell
{
    public int row, col;
    public int sofar;
    public sCell parent;
}

// Wrapper MonoBehaviour para que sCell pueda entrar en la PriorityQueue del asset
// (el asset requiere que TElement sea un Component)
public class sCellMono : MonoBehaviour
{
    public sCell cell;
}

public class Graph
{
    public List<List<eCellType>> mGrid = new List<List<eCellType>>();
    public int mCount;

    // La lista abierta ahora es una PriorityQueue del asset
    // TElement = sCellMono (Component), prioridad = f (int)
    private PriorityQueue<sCellMono> openQueue;
    private GameObject queueHost; // GameObject temporal que hostea los MonoBehaviours

    public List<sCell> ListaCerrada = new List<sCell>();
    private HashSet<(int, int)> inOpen = new HashSet<(int, int)>();

    Vector2Int start;
    Vector2Int goal;

    public void populateGrid(int size)
    {
        mCount = size;
        mGrid.Clear();

        for (int i = 0; i < size; i++)
        {
            mGrid.Add(new List<eCellType>());
            for (int j = 0; j < size; j++)
                mGrid[i].Add(eCellType.empty);
        }
    }

    public void Reset()
    {
        // Limpiar GameObjects temporales del paso anterior
        if (queueHost != null)
            GameObject.Destroy(queueHost);

        queueHost = new GameObject("_AStarQueue");
        openQueue = new PriorityQueue<sCellMono>();
        inOpen.Clear();
        ListaCerrada.Clear();

        for (int r = 0; r < mCount; r++)
            for (int c = 0; c < mCount; c++)
                if (mGrid[r][c] == eCellType.start || mGrid[r][c] == eCellType.goal)
                    mGrid[r][c] = eCellType.empty;
    }

    public void SetStart(Vector2Int s)
    {
        start = s;

        sCell cell = new sCell();
        cell.row = s.x;
        cell.col = s.y;
        cell.sofar = 0;
        cell.parent = null;

        int h = ReturnHeuristica(s.x, s.y);
        EnqueueCell(cell, h); // f = 0 + h

        mGrid[s.x][s.y] = eCellType.start;
    }

    public void SetGoal(Vector2Int g)
    {
        goal = g;
        mGrid[g.x][g.y] = eCellType.goal;
    }

    public bool UpdateStep(GameObject[,] tiles)
    {
        if (openQueue.IsEmpty())
            return true;

        sCellMono mono = openQueue.Dequeue();
        sCell current = mono.cell;
        inOpen.Remove((current.row, current.col));
        ListaCerrada.Add(current);

        tiles[current.row, current.col]
            .GetComponent<Renderer>().material.color = Color.yellow;

        if (current.row == goal.x && current.col == goal.y)
            return true;

        TryAddCell(current.row - 1, current.col, current);
        TryAddCell(current.row + 1, current.col, current);
        TryAddCell(current.row, current.col - 1, current);
        TryAddCell(current.row, current.col + 1, current);

        return false;
    }

    void TryAddCell(int row, int col, sCell parent)
    {
        if (row < 0 || col < 0 || row >= mCount || col >= mCount)
            return;

        if (mGrid[row][col] == eCellType.blocked)
            return;

        if (inOpen.Contains((row, col)))
            return;

        if (ListaCerrada.Any(c => c.row == row && c.col == col))
            return;

        sCell cell = new sCell();
        cell.row = row;
        cell.col = col;
        cell.parent = parent;
        cell.sofar = parent.sofar + 10;

        int h = ReturnHeuristica(row, col);
        int f = cell.sofar + h;

        EnqueueCell(cell, f);
    }

    void EnqueueCell(sCell cell, int priority)
    {
        sCellMono mono = queueHost.AddComponent<sCellMono>();
        mono.cell = cell;
        openQueue.Enqueue(mono, priority);
        inOpen.Add((cell.row, cell.col));
    }

    public List<sCell> GetOptimalPath()
    {
        List<sCell> path = new List<sCell>();

        sCell current = ListaCerrada.LastOrDefault(
            c => c.row == goal.x && c.col == goal.y
        );

        while (current != null)
        {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }

    int ReturnHeuristica(int row, int col)
    {
        int dx = Mathf.Abs(goal.x - row);
        int dy = Mathf.Abs(goal.y - col);
        return 10 * (dx + dy);
    }
}