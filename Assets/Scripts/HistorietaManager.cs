using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class HistorietaManager : MonoBehaviour
{
    [Header("UI")]
    public Image panelImagen;
    public TMP_Text panelTexto;
    public Button btnSiguiente;

    [Header("Paneles de la historieta")]
    public Sprite[] paneles;
    [TextArea(2, 5)]
    public string[] textos;
    [TextArea(2, 5)]
    public string[] textosEN; // mismo orden que textos

    [Header("Escena siguiente")]
    public string nombreEscenaSiguiente = "Tutorial";

    private int panelActual = 0;
    private bool puedeAvanzar = true;

    void Start()
    {
        btnSiguiente.onClick.AddListener(Siguiente);
        Debug.Log("Idioma actual: " + Loc.Actual);
        MostrarPanel(0);
    }

    void OnEnable() { Loc.OnIdiomaCambiado += Refrescar; }
    void OnDisable() { Loc.OnIdiomaCambiado -= Refrescar; }

    void Update()
    {
        if (!puedeAvanzar) return;

        // Solo avanzar con click si NO se hizo click sobre un elemento de UI
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
            Siguiente();
    }

    void MostrarPanel(int indice)
    {
        if (indice >= paneles.Length)
        {
            SceneManager.LoadScene(nombreEscenaSiguiente);
            return;
        }

        panelActual = indice;

        if (panelImagen != null && paneles[indice] != null)
            panelImagen.sprite = paneles[indice];

        Refrescar();
    }

    // Actualiza texto y botón según panel actual e idioma
    void Refrescar()
    {
        if (panelTexto != null)
        {
            string es = panelActual < textos.Length ? textos[panelActual] : "";
            string en = panelActual < textosEN.Length ? textosEN[panelActual] : es;
            panelTexto.text = Loc.T(es, en);
        }
        else
            Debug.LogWarning("panelTexto es null — no está asignado en el Inspector");

        var btnLabel = btnSiguiente.GetComponentInChildren<TMP_Text>();
        if (btnLabel != null)
            btnLabel.text = (panelActual == paneles.Length - 1)
                ? Loc.T("¡Comenzar!", "Start!")
                : Loc.T("Siguiente >", "Next >");
    }

    public void Siguiente()
    {
        MostrarPanel(panelActual + 1);
    }
}