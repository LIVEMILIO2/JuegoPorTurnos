using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;

    List<Vector3> path = new();
    int index = 0;

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            CalculatePath();

        MoveAlongPath();
    }

    void CalculatePath()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        Vector2Int start = GraphCreator.Instance.WorldToGrid(transform.position);
        Vector2Int goal = GraphCreator.Instance.WorldToGrid(hit.point);

        Graph g = GraphCreator.Instance.graph;

        g.SetStartAndGoal(start.x, start.y, goal.x, goal.y);
        GraphCreator.Instance.UpdateTileColors();

        while (!g.Step())
        {
            GraphCreator.Instance.UpdateTileColors();
        }

        path.Clear();
        foreach (var cell in g.GetPath())
            path.Add(GraphCreator.Instance.GridToWorld(cell.row, cell.col));

        GraphCreator.Instance.PaintPath(g.GetPath());
        index = 0;
    }

    void MoveAlongPath()
    {
        if (path.Count == 0 || index >= path.Count) return;

        Vector3 target = path[index];
        target.y = 1f;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) < 0.1f)
            index++;
    }
}
