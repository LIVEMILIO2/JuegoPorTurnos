using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;

public enum eCellType
{
    none,
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
    public List<sCell> ListaCerrada = new List<sCell>();
    public List<sCell> ListaAbierta = new List<sCell>();
    public void populateGrid(int sqsize = 10)
    {
        mCount = sqsize;
        for (int i = 0; i < sqsize; i++)
        {
            mGrid.Add(new List<eCellType>());
            for (int j = 0; j < sqsize; j++)
            {
                mGrid[i].Add(eCellType.empty);
            }
        }

        //mGrid[1][1] = eCellType.blocked;
        //mGrid[2][5] = eCellType.blocked;
        //mGrid[3][6] = eCellType.blocked;
        //mGrid[5][6] = eCellType.blocked;
        //mGrid[6][7] = eCellType.blocked;
        //mGrid[7][8] = eCellType.blocked;
        //mGrid[8][9] = eCellType.blocked;
        //mGrid[8][8] = eCellType.blocked;
        //mGrid[8][10] = eCellType.blocked;
        mGrid[0][0] = eCellType.start;
        mGrid[sqsize - 1][sqsize - 1] = eCellType.goal;

        sCell mcell = new sCell();
        mcell.row = 0;
        mcell.col = 0;
        mcell.sofar = 0;
        mcell.parent = null;
        ListaAbierta.Add(mcell);
    }
    public List<sCell> GetOptimalPath()
    {
        List<sCell> optimalPath = new List<sCell>();
        sCell current = ListaCerrada.LastOrDefault();

        while (current != null)
        {
            optimalPath.Add(current);
            current = current.parent;
        }

        optimalPath.Reverse();
        return optimalPath;
    }
    public bool UpdateStep(GameObject[,] tiles)
    {
        int minVal = int.MaxValue;
        int openIdx = -1;

        for (int i = 0; i < ListaAbierta.Count; i++)
        {
            sCell currentCell = ListaAbierta[i];
            int lsofar = currentCell.sofar;
            int lheuritic = ReturnHeuristica(currentCell.row, currentCell.col);
            int F = lsofar + lheuritic;

            if (F < minVal)
            {
                openIdx = i;
                minVal = F;
            }
        }

        if (openIdx >= 0)
        {
            sCell celu = ListaAbierta[openIdx];
            ListaCerrada.Add(celu);
            ListaAbierta.Remove(celu);
            tiles[celu.row, celu.col].GetComponent<Renderer>().material.color = Color.yellow;

            if (mGrid[celu.row][celu.col] == eCellType.goal)
            {
                return true;
            }

            int lr = celu.row;
            int lc = celu.col;
            sCell[] myCells = new sCell[8];
            myCells[0] = GetCellOnGrid(lr - 1, lc - 1, celu);
            myCells[1] = GetCellOnGrid(lr - 1, lc, celu);
            myCells[2] = GetCellOnGrid(lr - 1, lc + 1, celu);
            myCells[3] = GetCellOnGrid(lr, lc - 1, celu);
            myCells[4] = GetCellOnGrid(lr, lc + 1, celu);
            myCells[5] = GetCellOnGrid(lr + 1, lc - 1, celu);
            myCells[6] = GetCellOnGrid(lr + 1, lc, celu);
            myCells[7] = GetCellOnGrid(lr + 1, lc + 1, celu);

            for (int i = 0; i < 8; i++)
            {
                if (myCells[i] != null)
                {
                    ListaAbierta.Add(myCells[i]);
                }
            }
        }

        return false;
    }




    sCell GetCellOnGrid(int row, int col, sCell parent)
    {
        if (row < 0 || row > (mCount - 1)) { return null; }
        if (col < 0 || col > (mCount - 1)) { return null; }
        if (mGrid[row][col] == eCellType.blocked) { return null; }

        for (int i = 0; i < ListaAbierta.Count; i++)
        {
            if (ListaAbierta[i].row == row && ListaAbierta[i].col == col) { return null; }
        }
        for (int i = 0; i < ListaCerrada.Count; i++)
        {
            if (ListaCerrada[i].row == row && ListaCerrada[i].col == col) { return null; }
        }
        Debug.Log($"Evaluando celda: ({row}, {col})");


        int parentsofar = parent == null ? 0 : parent.sofar;
        int localsofar = ReturnDistance(row, col, parent.row, parent.col);
        int totalsofar = parentsofar + localsofar;
        sCell celus = new sCell();
        celus.row = row;
        celus.col = col;
        celus.sofar = totalsofar;
        return celus;

    }
    public int ReturnHeuristica(int Row, int Col)
    {
        int resta = (mGrid.Count - 1) - (Row);
        int resta2 = (mGrid.Count - 1) - (Col);
        return 10 * (resta + resta2);
    }
    int ReturnDistance(int finraw, int fincol, int raw, int col)
    {
        int resta = Mathf.Abs(finraw - raw);
        int resta2 = Mathf.Abs(fincol - col);
        return (resta + resta2) < 2 ? 10 : 14;
    }
}