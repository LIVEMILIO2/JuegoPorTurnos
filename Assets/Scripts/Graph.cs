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

public class Graph
{
    public List<List<eCellType>> mGrid = new List<List<eCellType>>();
    public int mCount;

    public List<sCell> ListaAbierta = new List<sCell>();
    public List<sCell> ListaCerrada = new List<sCell>();

    Vector2Int start;
    Vector2Int goal;

    // =========================
    // GRID
    // =========================
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

    // =========================
    // A* CONTROL
    // =========================
    public void Reset()
    {
        ListaAbierta.Clear();
        ListaCerrada.Clear();

        // limpia start / goal viejos
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

        ListaAbierta.Add(cell);
        mGrid[s.x][s.y] = eCellType.start;
    }

    public void SetGoal(Vector2Int g)
    {
        goal = g;
        mGrid[g.x][g.y] = eCellType.goal;
    }

    // =========================
    // A* STEP
    // =========================
    public bool UpdateStep(GameObject[,] tiles)
    {
        if (ListaAbierta.Count == 0)
            return true;

        int bestIndex = 0;
        int bestF = int.MaxValue;

        for (int i = 0; i < ListaAbierta.Count; i++)
        {
            int g = ListaAbierta[i].sofar;
            int h = ReturnHeuristica(ListaAbierta[i].row, ListaAbierta[i].col);
            int f = g + h;

            if (f < bestF)
            {
                bestF = f;
                bestIndex = i;
            }
        }

        sCell current = ListaAbierta[bestIndex];
        ListaAbierta.RemoveAt(bestIndex);
        ListaCerrada.Add(current);

        tiles[current.row, current.col].GetComponent<Renderer>().material.color = Color.yellow;

        if (current.row == goal.x && current.col == goal.y)
            return true;

        int r = current.row;
        int c = current.col;

        TryAddCell(r - 1, c, current);
        TryAddCell(r + 1, c, current);
        TryAddCell(r, c - 1, current);
        TryAddCell(r, c + 1, current);

        return false;
    }

    // =========================
    // NEIGHBORS
    // =========================
    void TryAddCell(int row, int col, sCell parent)
    {
        if (row < 0 || col < 0 || row >= mCount || col >= mCount)
            return;

        if (mGrid[row][col] == eCellType.blocked)
            return;

        if (ListaAbierta.Any(c => c.row == row && c.col == col))
            return;

        if (ListaCerrada.Any(c => c.row == row && c.col == col))
            return;

        sCell cell = new sCell();
        cell.row = row;
        cell.col = col;
        cell.parent = parent;
        cell.sofar = parent.sofar + 10;

        ListaAbierta.Add(cell);
    }

    // =========================
    // PATH
    // =========================
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

    // =========================
    // HEURISTIC
    // =========================
    int ReturnHeuristica(int row, int col)
    {
        int dx = Mathf.Abs(goal.x - row);
        int dy = Mathf.Abs(goal.y - col);
        return 10 * (dx + dy);
    }
}
