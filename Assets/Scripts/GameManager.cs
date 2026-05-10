using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<PlayerScript> players = new List<PlayerScript>();
    public List<EnemyScript> enemies = new List<EnemyScript>();

    [Header("UI Turno")]
    public TMP_Text nombreText;
    public TMP_Text vidaText;
    public TMP_Text movimientoText;
    public TMP_Text ataqueText;

    private PlayerScript playerActual;
    private EnemyScript enemyActual;
    private PriorityQueue<MonoBehaviour> turnQueue = new PriorityQueue<MonoBehaviour>();

 
    private bool modoMovimiento = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    System.Collections.IEnumerator Start()
    {
        yield return null;
        InicializarRonda();
    }

    void Update()
    {
        if (playerActual != null && !playerActual.EstaMoviendose() && modoMovimiento)
            DetectarClickMovimiento();
    }



    void InicializarRonda()
    {
        ConstruirCola();
        ActivarSiguiente();
    }

    void ConstruirCola()
    {
        turnQueue = new PriorityQueue<MonoBehaviour>();

        foreach (var p in players)
            if (p != null) turnQueue.Enqueue(p, -p.iniciativa);

        foreach (var e in enemies)
            if (e != null) turnQueue.Enqueue(e, -e.iniciativa);
    }

    public void SiguienteTurno()
    {
        modoMovimiento = false;
        playerActual = null;
        enemyActual = null;
        ActionPanelUI.Instance?.OcultarPanel();
        GraphCreator.Instance?.ResetVisual();

        if (turnQueue.IsEmpty())
        {
            InicializarRonda();
            return;
        }

        ActivarSiguiente();
    }

    void ApagarIndicadores()
    {
        foreach (var p in players)
            if (p != null && p.indicadorTurno != null)
                p.indicadorTurno.SetActive(false);

        foreach (var e in enemies)
            if (e != null && e.indicadorTurno != null)
                e.indicadorTurno.SetActive(false);
    }

    void ActivarSiguiente()
    {
        ApagarIndicadores();
        while (!turnQueue.IsEmpty())
        {
            MonoBehaviour next = turnQueue.Dequeue();
            if (next == null) continue;

            if (next is PlayerScript p)
            {
                playerActual = p;
                p.ReiniciarTurno(); 
                ActualizarUI();
                return;
            }

            if (next is EnemyScript e)
            {
                enemyActual = e;
                e.TomarTurno();
                ActualizarUI();
                return;
            }
        }

        InicializarRonda();
    }


    public void ModificarIniciativa(PlayerScript player, int nuevaIniciativa)
    {
        player.iniciativa = nuevaIniciativa;
    }

    public void ModificarIniciativa(EnemyScript enemy, int nuevaIniciativa)
    {
        enemy.iniciativa = nuevaIniciativa;
    }

    public void ReconstruirColaActual()
    {
        List<MonoBehaviour> restantes = new List<MonoBehaviour>();
        while (!turnQueue.IsEmpty())
            restantes.Add(turnQueue.Dequeue());

        turnQueue = new PriorityQueue<MonoBehaviour>();

        foreach (var entity in restantes)
        {
            if (entity is PlayerScript p && p != null)
                turnQueue.Enqueue(p, -p.iniciativa);
            else if (entity is EnemyScript e && e != null)
                turnQueue.Enqueue(e, -e.iniciativa);
        }
    }

  

    public void ActivarModoMovimiento()
    {
        modoMovimiento = true;
    }

    void DetectarClickMovimiento()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (playerActual == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            modoMovimiento = false;
            Vector2Int start = GraphCreator.Instance.WorldToGrid(playerActual.transform.position);
            Vector2Int goal = GraphCreator.Instance.WorldToGrid(hit.point);
            GraphCreator.Instance.CalcularCamino(start, goal, playerActual);
        }
    }

    public PlayerScript JugadorActual() => playerActual;
    public EnemyScript EnemyActual() => enemyActual;
    public bool EsTurnoPlayer() => playerActual != null;
    public bool EsTurnoEnemy() => enemyActual != null;

  
    void ActualizarUI()
    {
        if (playerActual != null)
        {
            nombreText.text = playerActual.playerStats;
            vidaText.text = "Vida: " + playerActual.health;
            movimientoText.text = "Movimiento: " + playerActual.playerMoveRange;
            ataqueText.text = "Ataque: " + playerActual.damage;
        }
        else if (enemyActual != null)
        {
            nombreText.text = "Enemy";
            vidaText.text = "Vida: " + enemyActual.currentHealth;
            movimientoText.text = "Movimiento: " + enemyActual.enemyMoveRange;
            ataqueText.text = "Ataque: " + enemyActual.damage;
        }
    }

    public void BotonPasarTurno()
    {
        SiguienteTurno();
    }


    public void RemoveEnemy(EnemyScript enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (enemyActual == enemy)
        {
            enemyActual = null;
            SiguienteTurno();
        }
    }

    public void RemovePlayer(PlayerScript player)
    {
        players.Remove(player);

        if (players.Count == 0)
        {
            SceneManager.LoadScene("GameOver");
            return;
        }

        if (playerActual == player)
        {
            playerActual = null;
            SiguienteTurno();
        }
    }
}