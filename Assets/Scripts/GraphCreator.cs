using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphCreator : MonoBehaviour
{
    public GameObject prefabTile;
    [SerializeField] private int TileCount = 10;
    private Graph mGraph = new Graph();
    private GameObject[,] tiles;
    bool foundok = false;

    void Start()
    {
        mGraph.populateGrid(TileCount);
        var grid = mGraph.mGrid;
        int count = mGraph.mCount;

        float startX = -count / 2f;
        float startZ = -startX;


        tiles = new GameObject[count, count];

        GameObject tilesParent = new GameObject("Tiles");

        for (int rowi = 0; rowi < count; rowi++)
        {
            for (int coli = 0; coli < count; coli++)
            {
                float xx = startX + coli;
                float zz = startZ - rowi;
                GameObject tile = Instantiate(prefabTile,
                    new Vector3(xx, 0f, zz), prefabTile.transform.rotation, tilesParent.transform);

                tiles[rowi, coli] = tile;

                switch (grid[rowi][coli])
                {
                    case eCellType.blocked:
                        tile.GetComponent<Renderer>().material.color = Color.blue;
                        break;
                    case eCellType.start:
                        tile.GetComponent<Renderer>().material.color = Color.green;
                        break;
                    case eCellType.goal:
                        tile.GetComponent<Renderer>().material.color = Color.red;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    void Update()
    {
        if (!foundok)
        {
            foundok = mGraph.UpdateStep(tiles);
            if (foundok)
            {
                MarkOptimalPath();
            }
        }
    }

    private void MarkOptimalPath()
    {
        List<sCell> optimalPath = mGraph.GetOptimalPath();
        if (optimalPath.Count > 0)
        {
            foreach (sCell cell in optimalPath)
            {
                tiles[cell.row, cell.col].GetComponent<Renderer>().material.color = Color.cyan;
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún camino óptimo.");
        }
    }
}