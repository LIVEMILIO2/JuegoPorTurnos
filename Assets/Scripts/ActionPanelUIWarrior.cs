using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPanelUIWarrior : ActionPanelBase
{
    [Header("Panel raíz")]
    public GameObject actionPanel;

    [Header("Botones")]
    public Button btnMover;
    public Button btnAtacar;
    public Button btnHabilidad1;
    public Button btnDefender;
    public Button btnPasarTurno;

    [Header("Texto de estado")]
    public TMP_Text statusText;

    private PlayerScript miPlayer;

    void Start()
    {
        btnMover.onClick.RemoveAllListeners();
        btnAtacar.onClick.RemoveAllListeners();
        btnHabilidad1.onClick.RemoveAllListeners();
        btnDefender.onClick.RemoveAllListeners();
        btnPasarTurno.onClick.RemoveAllListeners();

        btnMover.onClick.AddListener(OnMover);
        btnAtacar.onClick.AddListener(OnAtacar);
        btnHabilidad1.onClick.AddListener(OnHabilidad1);
        btnDefender.onClick.AddListener(OnDefender);
        btnPasarTurno.onClick.AddListener(OnPasarTurno);

        OcultarPanel();
    }

    public override void MostrarPanel(PlayerScript player)
    {
        miPlayer = player;
        actionPanel.SetActive(true);
        RefrescarBotones(player);
    }

    public override void OcultarPanel()
    {
        miPlayer = null;
        actionPanel.SetActive(false);
    }

    public override void RefrescarBotones(PlayerScript player)
    {
        if (player == null) return;

        btnMover.interactable = !player.yaSeMovio && !player.EstaMoviendose();
        btnAtacar.interactable = !player.yaUsoAccion;
        btnHabilidad1.interactable = !player.yaUsoAccion;
        btnDefender.interactable = !player.yaUsoAccion;
        btnPasarTurno.interactable = true;

        string mov = player.yaSeMovio ? "<color=grey>Movimiento ✓</color>" : "<color=white>Movimiento disponible</color>";
        string accion = player.yaUsoAccion ? "<color=grey>Acción ✓</color>" : "<color=white>Acción disponible</color>";
        statusText.text = $"{mov}\n{accion}";
    }

    PlayerScript GetPlayer()
    {
        PlayerScript fromManager = GameManager.Instance.JugadorActual();
        return fromManager != null ? fromManager : miPlayer;
    }

    void OnMover()
    {
        PlayerScript player = GetPlayer();
        if (player == null) return;

        statusText.text = "Haz click en el tablero para moverte...";
        btnMover.interactable = false;

        Vector2Int origen = GraphCreator.Instance.WorldToGrid(player.transform.position);
        GraphCreator.Instance.MostrarRangoMovimiento(origen, player.playerMoveRange);
        GameManager.Instance.ActivarModoMovimiento();
    }

    void OnAtacar()
    {
        PlayerScript player = GetPlayer();
        if (player == null || player.yaUsoAccion) return;

        EnemyScript objetivo = EnemyMasCercano(player);
        if (objetivo == null) { statusText.text = "No hay enemigos en rango."; return; }

        objetivo.RecibirDamage(player.damage);
        Debug.Log($"{player.playerStats} ataca a {objetivo.name} por {player.damage}");
        UsarAccion(player);
    }

    void OnHabilidad1()
    {
        PlayerScript player = GetPlayer();
        if (player == null || player.yaUsoAccion) return;

        GameManager.Instance.ModificarIniciativa(player, player.iniciativa + 3);
        GameManager.Instance.ReconstruirColaActual();
        Debug.Log($"{player.playerStats} usa Acelerar: iniciativa ahora {player.iniciativa}");
        UsarAccion(player);
    }

    void OnDefender()
    {
        PlayerScript player = GetPlayer();
        if (player == null || player.yaUsoAccion) return;

        GameManager.Instance.ModificarIniciativa(player, player.iniciativa + 5);
        GameManager.Instance.ReconstruirColaActual();
        player.estaDefendiendo = true;
        Debug.Log($"{player.playerStats} se defiende");
        UsarAccion(player);
    }

    void OnPasarTurno()
    {
        PlayerScript player = GetPlayer();
        if (player == null) return;
        UsarAccion(player);
    }

    void UsarAccion(PlayerScript player)
    {
        player.yaUsoAccion = true;
        player.yaSeMovio = true;
        RefrescarBotones(player);
        GameManager.Instance.SiguienteTurno();
    }

    EnemyScript EnemyMasCercano(PlayerScript player)
    {
        EnemyScript mejor = null;
        float mejorDist = Mathf.Infinity;

        foreach (var e in GameManager.Instance.enemies)
        {
            if (e == null) continue;
            float d = Vector3.Distance(player.transform.position, e.transform.position);
            if (d < mejorDist) { mejorDist = d; mejor = e; }
        }

        return mejor != null && mejorDist <= player.rangoAtaque ? mejor : null;
    }
}