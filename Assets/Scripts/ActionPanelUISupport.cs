using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionPanelUISupport : ActionPanelBase
{
    [Header("Panel raíz")]
    public GameObject actionPanel;

    [Header("Botones")]
    public Button btnMover;
    public Button btnAtacar;
    public Button btnHabilidad1;
    public Button btnHabilidad2;
    public Button btnPasarTurno;

    [Header("Texto de estado")]
    public TMP_Text statusText;

    public float cantidadCura = 30f;

    private PlayerScript miPlayer;

    void Start()
    {
        btnMover.onClick.RemoveAllListeners();
        btnAtacar.onClick.RemoveAllListeners();
        btnHabilidad1.onClick.RemoveAllListeners();
        btnHabilidad2.onClick.RemoveAllListeners();
        btnPasarTurno.onClick.RemoveAllListeners();

        btnMover.onClick.AddListener(OnMover);
        btnAtacar.onClick.AddListener(OnAtacar);
        btnHabilidad1.onClick.AddListener(OnCurar);
        btnHabilidad2.onClick.AddListener(OnBuff);
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
        btnHabilidad2.interactable = !player.yaUsoAccion;
        btnPasarTurno.interactable = true;

        string mov = player.yaSeMovio ? "<color=grey>Movimiento [OK]</color>" : "<color=white>Movimiento disponible</color>";
        string accion = player.yaUsoAccion ? "<color=grey>Accion [OK]</color>" : "<color=white>Accion disponible</color>";
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
        statusText.text = "Selecciona un enemigo...";
        GameManager.Instance.ActivarSeleccionEnemigo(player.rangoAtaque, enemigo =>
        {
            enemigo.RecibirDamage(player.damage);
            UsarAccion(player);
        });
    }

    void OnCurar()
    {
        PlayerScript player = GetPlayer();
        if (player == null || player.yaUsoAccion) return;
        statusText.text = "Curar: selecciona un aliado...";
        GameManager.Instance.ActivarSeleccionAliado(player.rangoHabilidad, aliado =>
        {
            aliado.health = Mathf.Min(aliado.health + cantidadCura, aliado.healthMax);
            Debug.Log($"Support cura a {aliado.name} por {cantidadCura}. Vida: {aliado.health}");
            UsarAccion(player);
        });
    }

    void OnBuff()
    {
        PlayerScript player = GetPlayer();
        if (player == null || player.yaUsoAccion) return;
        statusText.text = "Buff: selecciona un aliado...";
        GameManager.Instance.ActivarSeleccionAliado(player.rangoHabilidad, aliado =>
        {
            GameManager.Instance.ModificarIniciativa(aliado, aliado.iniciativa + 3);
            GameManager.Instance.ReconstruirColaActual();
            Debug.Log($"Support buffea a {aliado.name}: iniciativa ahora {aliado.iniciativa}");
            UsarAccion(player);
        });
    }

    void OnPasarTurno()
    {
        PlayerScript player = GetPlayer();
        if (player == null) return;
        player.yaSeMovio = true;
        player.yaUsoAccion = true;
        RefrescarBotones(player);
        GameManager.Instance.SiguienteTurno();
    }

    void UsarAccion(PlayerScript player)
    {
        player.yaUsoAccion = true;
        RefrescarBotones(player);
        if (player.yaSeMovio)
            GameManager.Instance.SiguienteTurno();
    }
}