using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerScript> players = new List<PlayerScript>();
    public List<EnemyScript> enemies = new List<EnemyScript>();

    [Header("Transición de nivel")]
    //blic string nombreEscenaSiguiente; 
    public float tiempoEsperaAntesDeCargar = 1f; 

    private int turnoIndex = 0;

    private enum TurnState { PlayerTurn, EnemyTurn }
    private TurnState currentTurnState;

    void Awake()
   {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InicializarTurnos();
    }

    void Update()
    {
        if (EsTurnoPlayer() && JugadorActual() != null && !JugadorActual().EstaMoviendose())
        {
            DetectarClickMovimiento();
        }
    }

    void InicializarTurnos()
    {
        turnoIndex = 0;
        if (players.Count + enemies.Count == 0)
        {
            Debug.LogWarning("No hay jugadores ni enemigos en la escena.");
            return;
        }
        if (players.Count > 0)
        {
            currentTurnState = TurnState.PlayerTurn;
            JugadorActual()?.ReiniciarTurno();
        }
        else
        {
            currentTurnState = TurnState.EnemyTurn;
            EnemyActual()?.TomarTurno();
        }
    }

    void DetectarClickMovimiento()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        PlayerScript player = JugadorActual();
        if (player == null)
            return;

        if (player.EstaMoviendose())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 destino = hit.point;

            if (GraphCreator.Instance == null)
            {
                Debug.LogError("GraphCreator.Instance no está asignado.");
                return;
            }

            Vector2Int start = GraphCreator.Instance.WorldToGrid(player.transform.position);
            Vector2Int goal = GraphCreator.Instance.WorldToGrid(destino);

            GraphCreator.Instance.CalcularCamino(start, goal, player);
        }
    }

    public PlayerScript JugadorActual()
    {
        if (turnoIndex < players.Count)
            return players[turnoIndex];
        return null;
    }

    public EnemyScript EnemyActual()
    {
        int index = turnoIndex - players.Count;
        if (index >= 0 && index < enemies.Count)
            return enemies[index];
        return null;
    }

    public bool EsTurnoPlayer()
    {
        return turnoIndex < players.Count;
    }

    public bool EsTurnoEnemy()
    {
        return turnoIndex >= players.Count && turnoIndex < players.Count + enemies.Count;
    }

    public void SiguienteTurno()
    {
        int totalPersonajes = players.Count + enemies.Count;

        if (totalPersonajes == 0)
        {
            turnoIndex = 0;
            Debug.LogWarning("No hay personajes para gestionar turnos.");
            return;
        }
        turnoIndex++;
        if (turnoIndex >= totalPersonajes)
            turnoIndex = 0;

        if (EsTurnoPlayer())
        {
            currentTurnState = TurnState.PlayerTurn;
            PlayerScript player = JugadorActual();
            if (player != null)
            {
                player.ReiniciarTurno();
            }
            else
            {
                Debug.LogWarning("Jugador actual es null, saltando turno.");
                SiguienteTurno();
            }
        }
        else if (EsTurnoEnemy())
        {
            currentTurnState = TurnState.EnemyTurn;
            EnemyScript enemy = EnemyActual();
            if (enemy != null)
            {
                enemy.TomarTurno();
            }
            else
            {
                Debug.LogWarning("Enemigo actual es null, saltando turno.");
                SiguienteTurno();
            }
        }
        else
        {
            Debug.LogError("Índice de turno fuera de rango, reiniciando.");
            turnoIndex = 0;
            SiguienteTurno();
        }
    }

    public void BotonPasarTurno()
    {
        Debug.Log("BOTÓN PASAR TURNO PRESIONADO");

        PlayerScript jugador = JugadorActual();
        if (jugador != null && jugador.EstaMoviendose())
        {
     
        }

        SiguienteTurno();
        GraphCreator.Instance?.ResetVisual();
    }

    public void RecalcularTurnos()
    {
        turnoIndex = 0;
        if (players.Count > 0)
            players[0].ReiniciarTurno();
        else if (enemies.Count > 0)
            enemies[0].TomarTurno();
        else
            Debug.LogWarning("No hay personajes después de recalcular.");
    }

    public void RemoveEnemy(EnemyScript enemy)
    {
        if (enemies.Contains(enemy))
        {
            int index = enemies.IndexOf(enemy);
            enemies.RemoveAt(index);
            AjustarTurnoTrasEliminacion(index, esJugador: false);

            // Si ya no quedan enemigos, cargar siguiente escena
            if (enemies.Count == 0)
            {
                SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
    public void RemovePlayer(PlayerScript player)
    {
        if (players.Contains(player))
        {
            int index = players.IndexOf(player);
            players.RemoveAt(index);
            AjustarTurnoTrasEliminacion(index, esJugador: true);

            if (players.Count == 0)
                SceneManager.LoadScene("GameOver"); 
        }
    }
    private void AjustarTurnoTrasEliminacion(int indiceEliminado, bool esJugador)
    {
        if (turnoIndex > indiceEliminado)
        {
            turnoIndex--;
        }

        else if (turnoIndex == indiceEliminado)
        {

            if (turnoIndex >= players.Count + enemies.Count)
                turnoIndex = 0;

            if (EsTurnoPlayer())
                JugadorActual()?.ReiniciarTurno();
            else
                EnemyActual()?.TomarTurno();
        }
    }

    //private void IniciarCargaDeSiguienteEscena()
    //{
    //    if (!string.IsNullOrEmpty(nombreEscenaSiguiente))
    //    {
    //        StartCoroutine(CargarEscenaConRetraso());
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No has asignado un nombre de escena siguiente en el GameManager.");
    //    }
    //}

    //private IEnumerator CargarEscenaConRetraso()
    //{
    //    Debug.Log("¡Todos los enemigos derrotados! Cargando siguiente nivel...");
    //    yield return new WaitForSeconds(tiempoEsperaAntesDeCargar);
    //    SceneManager.LoadScene(sceneBuildIndex: SceneManager.GetActiveScene().buildIndex + 1);
    //}
}