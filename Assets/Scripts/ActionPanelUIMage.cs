using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPanelUIMage : ActionPanelBase
{
    [Header("Panel raíz")]
    public GameObject actionPanel;

    [Header("Botones")]
    public Button btnMover;
    public Button btnAtacar;
    public Button btnHabilidad1;
    public Button btnPasarTurno;

    [Header("Texto de estado")]
    public TMP_Text statusText;

    void Start()
    {
        btnMover.onClick.AddListener(OnMover);
        btnAtacar.onClick.AddListener(OnAtacar);
        btnHabilidad1.onClick.AddListener(OnHabilidad1);
        btnPasarTurno.onClick.AddListener(OnPasarTurno);

        OcultarPanel();
    }

    public override void MostrarPanel(PlayerScript player)
    {
        actionPanel.SetActive(true);
        RefrescarBotones(player);
    }

    public override void OcultarPanel()
    {
        actionPanel.SetActive(false);
    }

    public override void RefrescarBotones(PlayerScript player)
    {
        if (player == null) return;

        btnMover.interactable = !player.yaSeMovio && !player.EstaMoviendose();
        btnAtacar.interactable = !player.yaUsoAccion;
        btnHabilidad1.interactable = !player.yaUsoAccion;
        btnPasarTurno.interactable = true;

        string mov = player.yaSeMovio ? "<color=grey>Movimiento ✓</color>" : "<color=white>Movimiento disponible</color>";
        string accion = player.yaUsoAccion ? "<color=grey>Acción ✓</color>" : "<color=white>Acción disponible</color>";
        statusText.text = $"{mov}\n{accion}";
    }

    void OnMover()
    {
        PlayerScript player = GameManager.Instance.JugadorActual();
        if (player == null) return;

        statusText.text = "Haz click en el tablero para moverte...";
        btnMover.interactable = false;

        Vector2Int origen = GraphCreator.Instance.WorldToGrid(player.transform.position);
        GraphCreator.Instance.MostrarRangoMovimiento(origen, player.playerMoveRange);
        GameManager.Instance.ActivarModoMovimiento();
    }

    void OnAtacar()
    {
        PlayerScript player = GameManager.Instance.JugadorActual();
        if (player == null || player.yaUsoAccion) return;

        EnemyScript objetivo = EnemyMasCercano(player);
        if (objetivo == null) { statusText.text = "No hay enemigos en rango."; return; }

        objetivo.RecibirDamage(player.damage);
        UsarAccion(player);
    }

    void OnHabilidad1()
    {
        // Stun: -5 iniciativa al enemy más cercano
        PlayerScript player = GameManager.Instance.JugadorActual();
        if (player == null || player.yaUsoAccion) return;

        EnemyScript objetivo = EnemyMasCercano(player);
        if (objetivo == null) { statusText.text = "No hay enemigos en rango de Stun."; return; }

        GameManager.Instance.ModificarIniciativa(objetivo, objetivo.iniciativa - 5);
        GameManager.Instance.ReconstruirColaActual();
        UsarAccion(player);
    }

    void OnPasarTurno() => GameManager.Instance.BotonPasarTurno();

    void UsarAccion(PlayerScript player)
    {
        player.yaUsoAccion = true;
        RefrescarBotones(player);
        if (player.yaSeMovio) GameManager.Instance.SiguienteTurno();
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