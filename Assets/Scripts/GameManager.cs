using UnityEngine;

public enum Turno
{
    Player1,
    Player2
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerScript player1;
    public PlayerScript player2;

    public Turno turnoActual = Turno.Player1;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        PlayerScript activo = GetPlayerActivo();
        if (activo.EstaMoviendose())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorld = GetMouseWorldPosition();
            Vector2Int start = GraphCreator.Instance.WorldToGrid(activo.transform.position);
            Vector2Int goal = GraphCreator.Instance.WorldToGrid(clickWorld);

            GraphCreator.Instance.CalcularCamino(start, goal, activo);
        }
    }

    PlayerScript GetPlayerActivo()
    {
        return turnoActual == Turno.Player1 ? player1 : player2;
    }

    public void SiguienteTurno()
    {
        turnoActual = turnoActual == Turno.Player1 ? Turno.Player2 : Turno.Player1;
        Debug.Log("Turno de " + turnoActual);
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.point;
        return Vector3.zero;
    }
}
