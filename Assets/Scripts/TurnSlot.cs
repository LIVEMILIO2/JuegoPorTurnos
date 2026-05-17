using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Un slot de la barra de turnos, sin sprites.
/// 
/// Estructura del prefab (todo UI):
///   TurnSlot  (RectTransform 60x60 + Image fondo + este script)
///     └── Borde  (Image — outline, desactivado por defecto)
///     └── Nombre (TMP_Text opcional, para mostrar inicial)
/// </summary>
public class TurnSlot : MonoBehaviour
{
    [Header("Referencias")]
    public Image fondo;
    public Image borde;
    public TMP_Text label;   // Opcional: muestra "P" o "E"

    [Header("Colores")]
    public Color colorPlayer = new Color(0.2f, 0.5f, 1f);
    public Color colorEnemy = new Color(0.9f, 0.2f, 0.2f);
    public Color colorBorde = Color.yellow;

    public void Setup(bool esPlayer, bool esActivo)
    {
        fondo.color = esPlayer ? colorPlayer : colorEnemy;

        if (borde != null)
        {
            borde.gameObject.SetActive(esActivo);
            borde.color = colorBorde;
        }

        if (label != null)
            label.text = esPlayer ? "P" : "E";

        // El activo escala un poco más grande
        transform.localScale = esActivo ? Vector3.one * 1.15f : Vector3.one;
    }
}