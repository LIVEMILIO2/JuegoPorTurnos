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
    public Button btnHabilidad1; // Curar
    public Button btnHabilidad2; // Buff
    public Button btnPasarTurno;

    [Header("Texto de estado")]
    public TMP_Text statusText;

    public float cantidadCura = 30f;

    private PlayerScript miPlayer;
    private bool enTutorial => TutorialManager.Instance != null;

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

        // En tutorial, ModoTutorial controla los botones
        if (enTutorial) return;

        btnMover.interactable = !player.yaSeMovio && !player.EstaMoviendose();
        btnAtacar.interactable = !player.yaUsoAccion;
        btnHabilidad1.interactable = !player.yaUsoAccion;
        btnHabilidad2.interactable = !player.yaUsoAccion;
        btnPasarTurno.interactable = true;

        string mov = player.yaSeMovio ? "<color=grey>Movimiento [OK]</color>" : "<color=white>Movimiento disponible</color>";
        string accion = player.yaUsoAccion ? "<color=grey>Accion [OK]</color>" : "<color=white>Accion disponible</color>";
        statusText.text = $"{mov}\n{accion}";
    }

    public override void ModoTutorial(TutorialManager.AccionEsperada accion, string personaje)
    {
        if (personaje != "Support" || accion == TutorialManager.AccionEsperada.Ninguna)
        {
            btnMover.interactable = false;
            btnAtacar.interactable = false;
            btnHabilidad1.interactable = false;
            btnHabilidad2.interactable = false;
            btnPasarTurno.interactable = false;
            return;
        }

        btnMover.interactable = accion == TutorialManager.AccionEsperada.Moverse;
        btnAtacar.interactable = accion == TutorialManager.AccionEsperada.Atacar;
        btnHabilidad1.interactable = accion == TutorialManager.AccionEsperada.Habilidad1;
        btnHabilidad2.interactable = accion == TutorialManager.AccionEsperada.Habilidad2;
        btnPasarTurno.interactable = false;
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
            UsarAccion(player, TutorialManager.AccionEsperada.Atacar);
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
            UsarAccion(player, TutorialManager.AccionEsperada.Habilidad1);
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
            UsarAccion(player, TutorialManager.AccionEsperada.Habilidad2);
        });
    }

    void OnPasarTurno()
    {
        PlayerScript player = GetPlayer();
        if (player == null) return;
        player.yaSeMovio = true;
        player.yaUsoAccion = true;
        GameManager.Instance.SiguienteTurno();
    }

    void UsarAccion(PlayerScript player, TutorialManager.AccionEsperada accionHecha)
    {
        player.yaUsoAccion = true;

        if (enTutorial)
        {
            bool esUltimo = TutorialManager.Instance.EsUltimoPasoDelPersonaje();
            TutorialManager.Instance.AccionCompletada(accionHecha);
            if (player.yaSeMovio && esUltimo)
                GameManager.Instance.SiguienteTurno();
        }
        else if (player.yaSeMovio)
        {
            GameManager.Instance.SiguienteTurno();
        }
    }
}