using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerScript> players = new List<PlayerScript>();
    public List<EnemyScript> enemies = new List<EnemyScript>();
    public List<Transform> turnPos = new List<Transform>();

    [Header("UI Turno")]
    public TMP_Text nombreText;
    public TMP_Text vidaText;
    public TMP_Text movimientoText;
    public TMP_Text ataqueText;

    [Header("Transición de nivel")]
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

        ActualizarUI();
    }

    void DetectarClickMovimiento()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        PlayerScript player = JugadorActual();
        if (player == null) return;

        if (player.EstaMoviendose()) return;

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
        int total = players.Count + enemies.Count;

        if (total == 0)
        {
            turnoIndex = 0;
            return;
        }

        turnoIndex++;

        if (turnoIndex >= total)
            turnoIndex = 0;

        if (EsTurnoPlayer())
        {
            currentTurnState = TurnState.PlayerTurn;

            PlayerScript p = JugadorActual();
            if (p != null)
                p.ReiniciarTurno();
            else
                SiguienteTurno();
        }
        else if (EsTurnoEnemy())
        {
            currentTurnState = TurnState.EnemyTurn;

            EnemyScript e = EnemyActual();
            if (e != null)
                e.TomarTurno();
            else
                SiguienteTurno();
        }

        ActualizarUI();
    }
    void ActualizarUI()
    {
        if (EsTurnoPlayer())
        {
            PlayerScript p = JugadorActual();
            if (p == null) return;

            nombreText.text = p.playerStats;
            vidaText.text = "Vida: " + p.health;
            movimientoText.text = "Movimiento: " + p.playerMoveRange;
            ataqueText.text = "Ataque: " + p.damage;
        }
        else if (EsTurnoEnemy())
        {
            EnemyScript e = EnemyActual();
            if (e == null) return;

            nombreText.text = "Enemy";
            vidaText.text = "Vida: " + e.currentHealth;
            movimientoText.text = "Movimiento: " + e.enemyMoveRange;
            ataqueText.text = "Ataque: " + e.damage;
        }
    }

    public void BotonPasarTurno()
    {
        SiguienteTurno();
        GraphCreator.Instance?.ResetVisual();
    }

    public void RemoveEnemy(EnemyScript enemy)
    {
        if (enemies.Contains(enemy))
        {
            int index = enemies.IndexOf(enemy);
            enemies.RemoveAt(index);

            AjustarTurnoTrasEliminacion(index, false);

            if (enemies.Count == 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    public void RemovePlayer(PlayerScript player)
    {
        if (players.Contains(player))
        {
            int index = players.IndexOf(player);
            players.RemoveAt(index);

            AjustarTurnoTrasEliminacion(index, true);

            if (players.Count == 0)
                SceneManager.LoadScene("GameOver");
        }
    }

    private void AjustarTurnoTrasEliminacion(int eliminado, bool esJugador)
    {
        if (turnoIndex > eliminado)
        {
            turnoIndex--;
        }
        else if (turnoIndex == eliminado)
        {
            if (turnoIndex >= players.Count + enemies.Count)
                turnoIndex = 0;

            if (EsTurnoPlayer())
                JugadorActual()?.ReiniciarTurno();
            else
                EnemyActual()?.TomarTurno();
        }

        ActualizarUI();
    }
}