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

    [Header("Escena siguiente")]
    public string nombreEscenaSiguiente = "Tutorial";

    private int panelActual = 0;
    private bool puedeAvanzar = true;

    void Start()
    {
        btnSiguiente.onClick.AddListener(Siguiente);
        MostrarPanel(0);
    }

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

        if (panelTexto != null)
            panelTexto.text = indice < textos.Length ? textos[indice] : "";

        var btnLabel = btnSiguiente.GetComponentInChildren<TMP_Text>();
        if (btnLabel != null)
            btnLabel.text = (indice == paneles.Length - 1) ? "¡Comenzar!" : "Siguiente >";
        if (panelTexto != null)
        {
            panelTexto.text = indice < textos.Length ? textos[indice] : "";
            Debug.Log($"Texto panel {indice}: '{panelTexto.text}'");
        }
        else
            Debug.LogWarning("panelTexto es null — no está asignado en el Inspector");
    }

    public void Siguiente()
    {
        MostrarPanel(panelActual + 1);
    }
}