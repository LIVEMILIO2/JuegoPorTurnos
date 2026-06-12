using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public enum ModoSeleccion { Ninguno, Enemigo, Aliado }

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

    private ModoSeleccion modoSeleccion = ModoSeleccion.Ninguno;
    private float rangoSeleccion = 0f;
    private System.Action<EnemyScript> callbackEnemigo;
    private System.Action<PlayerScript> callbackAliado;

    private List<TurnEntry> ordenActual = new List<TurnEntry>();

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

        if (modoSeleccion != ModoSeleccion.Ninguno)
            DetectarClickTarget();
    }

    // ─── Selección de target ─────────────────────────────────────────────────

    public void ActivarSeleccionEnemigo(float rango, System.Action<EnemyScript> callback)
    {
        modoSeleccion = ModoSeleccion.Enemigo;
        rangoSeleccion = rango;
        callbackEnemigo = callback;
        callbackAliado = null;

        foreach (var e in enemies)
        {
            if (e == null || e.indicadorTurno == null) continue;
            float dist = Vector3.Distance(playerActual.transform.position, e.transform.position);
            e.indicadorTurno.SetActive(dist <= rango);
        }
    }

    public void ActivarSeleccionAliado(float rango, System.Action<PlayerScript> callback)
    {
        modoSeleccion = ModoSeleccion.Aliado;
        rangoSeleccion = rango;
        callbackAliado = callback;
        callbackEnemigo = null;

        foreach (var p in players)
        {
            if (p == null || p.indicadorTurno == null || p == playerActual) continue;
            float dist = Vector3.Distance(playerActual.transform.position, p.transform.position);
            p.indicadorTurno.SetActive(dist <= rango);
        }
    }

    public void CancelarSeleccion()
    {
        modoSeleccion = ModoSeleccion.Ninguno;
        rangoSeleccion = 0f;
        callbackEnemigo = null;
        callbackAliado = null;
        ApagarIndicadores();
        if (playerActual != null && playerActual.indicadorTurno != null)
            playerActual.indicadorTurno.SetActive(true);
    }

    void DetectarClickTarget()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit)) return;

        if (modoSeleccion == ModoSeleccion.Enemigo)
        {
            EnemyScript enemy = hit.collider.GetComponentInParent<EnemyScript>();
            if (enemy != null && enemies.Contains(enemy))
            {
                float dist = Vector3.Distance(playerActual.transform.position, enemy.transform.position);
                if (dist <= rangoSeleccion)
                {
                    var cb = callbackEnemigo;
                    CancelarSeleccion();
                    cb?.Invoke(enemy);
                }
            }
        }
        else if (modoSeleccion == ModoSeleccion.Aliado)
        {
            PlayerScript player = hit.collider.GetComponentInParent<PlayerScript>();
            if (player != null && players.Contains(player) && player != playerActual)
            {
                float dist = Vector3.Distance(playerActual.transform.position, player.transform.position);
                if (dist <= rangoSeleccion)
                {
                    var cb = callbackAliado;
                    CancelarSeleccion();
                    cb?.Invoke(player);
                }
            }
        }
    }

    // ─── Turnos ──────────────────────────────────────────────────────────────

    void InicializarRonda()
    {
        ConstruirCola();
        RefrescarOrdenBarra();
        ActivarSiguiente();
    }

    void ConstruirCola()
    {
        turnQueue = new PriorityQueue<MonoBehaviour>();
        foreach (var p in players) if (p != null) turnQueue.Enqueue(p, -p.iniciativa);
        foreach (var e in enemies) if (e != null) turnQueue.Enqueue(e, -e.iniciativa);
    }

    void RefrescarOrdenBarra()
    {
        ordenActual.Clear();
        var todos = new List<(MonoBehaviour mb, int ini, bool esPlayer)>();
        foreach (var p in players) if (p != null) todos.Add((p, p.iniciativa, true));
        foreach (var e in enemies) if (e != null) todos.Add((e, e.iniciativa, false));
        todos.Sort((a, b) => b.ini.CompareTo(a.ini));
        foreach (var t in todos)
        {
            Sprite spr = t.esPlayer ? ((PlayerScript)t.mb).portrait : ((EnemyScript)t.mb).portrait;
            ordenActual.Add(new TurnEntry { entidad = t.mb, portrait = spr, esPlayer = t.esPlayer });
        }
        MonoBehaviour activo = (MonoBehaviour)playerActual ?? enemyActual;
        TurnBarUI.Instance?.Refrescar(ordenActual, activo);
    }

    public void SiguienteTurno()
    {
        if (enemyActual != null && enemyActual.Moving()) return;
        if (playerActual != null && playerActual.EstaMoviendose()) return;

        CancelarSeleccion();
        modoMovimiento = false;
        playerActual?.OcultarPanel();
        playerActual = null;
        enemyActual = null;
        GraphCreator.Instance?.ResetVisual();

        if (turnQueue.IsEmpty()) { InicializarRonda(); return; }
        ActivarSiguiente();
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
                ActualizarUI();
                RefrescarOrdenBarra();
                p.ReiniciarTurno();
                TutorialManager.Instance?.NotificarCambioTurno();
                return;
            }

            if (next is EnemyScript e)
            {
                enemyActual = e;
                ActualizarUI();
                RefrescarOrdenBarra();
                e.TomarTurno();
                return;
            }
        }

        InicializarRonda();
    }

    void ApagarIndicadores()
    {
        foreach (var p in players) if (p != null && p.indicadorTurno != null) p.indicadorTurno.SetActive(false);
        foreach (var e in enemies) if (e != null && e.indicadorTurno != null) e.indicadorTurno.SetActive(false);
    }

    public void ModificarIniciativa(PlayerScript player, int nuevaIniciativa) => player.iniciativa = nuevaIniciativa;
    public void ModificarIniciativa(EnemyScript enemy, int nuevaIniciativa) => enemy.iniciativa = nuevaIniciativa;

    public void ReconstruirColaActual()
    {
        var restantes = new List<MonoBehaviour>();
        while (!turnQueue.IsEmpty()) restantes.Add(turnQueue.Dequeue());
        turnQueue = new PriorityQueue<MonoBehaviour>();
        foreach (var entity in restantes)
        {
            if (entity is PlayerScript p && p != null) turnQueue.Enqueue(p, -p.iniciativa);
            else if (entity is EnemyScript e && e != null) turnQueue.Enqueue(e, -e.iniciativa);
        }
        RefrescarOrdenBarra();
    }

    public void ActivarModoMovimiento() => modoMovimiento = true;

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
        // Cada panel maneja su propio pasar turno
    }

    public void RemoveEnemy(EnemyScript enemy)
    {
        enemies.Remove(enemy);
        RefrescarOrdenBarra();
        if (enemies.Count == 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        if (enemyActual == enemy) { enemyActual = null; SiguienteTurno(); }
    }

    public void RemovePlayer(PlayerScript player)
    {
        players.Remove(player);
        RefrescarOrdenBarra();
        if (players.Count == 0) { SceneManager.LoadScene("GameOver"); return; }
        if (playerActual == player) { playerActual = null; SiguienteTurno(); }
    }
}