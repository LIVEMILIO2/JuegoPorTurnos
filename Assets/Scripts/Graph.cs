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
    public List<List<eCellType>> mGrid = new();
    public int mCount;

    public List<sCell> ListaAbierta = new();
    public List<sCell> ListaCerrada = new();

    public void PopulateGrid(int size)
    {
        mCount = size;
        mGrid.Clear();

        for (int r = 0; r < size; r++)
        {
            mGrid.Add(new List<eCellType>());
            for (int c = 0; c < size; c++)
                mGrid[r].Add(eCellType.empty);
        }
    }

    public void SetStartAndGoal(int sr, int sc, int gr, int gc)
    {
        ListaAbierta.Clear();
        ListaCerrada.Clear();

        for (int r = 0; r < mCount; r++)
            for (int c = 0; c < mCount; c++)
                if (mGrid[r][c] != eCellType.blocked)
                    mGrid[r][c] = eCellType.empty;

        mGrid[sr][sc] = eCellType.start;
        mGrid[gr][gc] = eCellType.goal;

        ListaAbierta.Add(new sCell
        {
            row = sr,
            col = sc,
            sofar = 0,
            parent = null
        });
    }

    public bool Step()
    {
        if (ListaAbierta.Count == 0) return true;

        sCell current = ListaAbierta
            .OrderBy(c => c.sofar + Heuristic(c.row, c.col))
            .First();

        ListaAbierta.Remove(current);
        ListaCerrada.Add(current);

        if (mGrid[current.row][current.col] == eCellType.goal)
            return true;

        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                AddNeighbor(current, current.row + dr, current.col + dc);
            }

        return false;
    }

    void AddNeighbor(sCell parent, int r, int c)
    {
        if (r < 0 || c < 0 || r >= mCount || c >= mCount) return;
        if (mGrid[r][c] == eCellType.blocked) return;
        if (ListaAbierta.Any(x => x.row == r && x.col == c)) return;
        if (ListaCerrada.Any(x => x.row == r && x.col == c)) return;

        int cost = (Mathf.Abs(parent.row - r) + Mathf.Abs(parent.col - c) == 2) ? 14 : 10;

        ListaAbierta.Add(new sCell
        {
            row = r,
            col = c,
            sofar = parent.sofar + cost,
            parent = parent
        });
    }

    int Heuristic(int r, int c)
    {
        int gr = mGrid.FindIndex(x => x.Contains(eCellType.goal));
        int gc = mGrid[gr].IndexOf(eCellType.goal);
        return 10 * (Mathf.Abs(gr - r) + Mathf.Abs(gc - c));
    }

    public List<sCell> GetPath()
    {
        List<sCell> path = new();
        sCell cur = ListaCerrada.LastOrDefault();

        while (cur != null)
        {
            path.Add(cur);
            cur = cur.parent;
        }

        path.Reverse();
        return path;
    }
}
