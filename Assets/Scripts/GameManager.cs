using UnityEngine;

public enum Turno
{
    Player1,
    Player2,
    Enemy
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public PlayerScript player1;
    public PlayerScript player2;
    public EnemyScript enemy;

    public Turno turnoActual = Turno.Player1;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (turnoActual == Turno.Enemy)
            return;

        PlayerScript activo = GetPlayerActivo();

        if (activo.EstaMoviendose())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickWorld = GetMouseWorldPosition();

            Vector2Int start =
                GraphCreator.Instance.WorldToGrid(activo.transform.position);

            Vector2Int goal =
                GraphCreator.Instance.WorldToGrid(clickWorld);

            GraphCreator.Instance.CalcularCamino(start, goal, activo);
        }
    }

    PlayerScript GetPlayerActivo()
    {
        if (turnoActual == Turno.Player1)
            return player1;

        if (turnoActual == Turno.Player2)
            return player2;

        return null;
    }

    public void SiguienteTurno()
    {
        if (turnoActual == Turno.Player1)
        {
            turnoActual = Turno.Player2;

            player2.ReiniciarTurno();
        }
        else if (turnoActual == Turno.Player2)
        {
            turnoActual = Turno.Enemy;

            TurnoEnemy();
        }
        else
        {
            turnoActual = Turno.Player1;

            player1.ReiniciarTurno();
        }

        Debug.Log("Turno de " + turnoActual);
    }

    //  ESTA ES LA FUNCION DEL BOTON
    public void BotonPasarTurno()
    {
        Debug.Log("BOTON PASAR TURNO PRESIONADO");

        SiguienteTurno();
    }

    void TurnoEnemy()
    {
        PlayerScript objetivo = GetPlayerMasCercano();

        Vector2Int start =
            GraphCreator.Instance.WorldToGrid(enemy.transform.position);

        Vector2Int goal =
            GraphCreator.Instance.WorldToGrid(objetivo.transform.position);

        GraphCreator.Instance.CalcularCaminoEnemy(start, goal, enemy);
    }

    PlayerScript GetPlayerMasCercano()
    {
        float d1 =
            Vector3.Distance(enemy.transform.position, player1.transform.position);

        float d2 =
            Vector3.Distance(enemy.transform.position, player2.transform.position);

        return d1 <= d2 ? player1 : player2;
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray =
            Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
            return hit.point;

        return Vector3.zero;
    }
}