using UnityEngine;
using TMPro;

/// <summary>
/// Ponlo en cualquier TMP_Text fijo (botones, títulos). Si "es" está vacío,
/// toma el texto que ya tiene el TMP como versión en español.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [TextArea] public string es;
    [TextArea] public string en;

    private TMP_Text txt;

    void Awake()
    {
        txt = GetComponent<TMP_Text>();
        if (string.IsNullOrEmpty(es)) es = txt.text;
    }

    void OnEnable()
    {
        Loc.OnIdiomaCambiado += Refrescar;
        Refrescar();
    }

    void OnDisable()
    {
        Loc.OnIdiomaCambiado -= Refrescar;
    }

    void Refrescar()
    {
        if (txt == null) txt = GetComponent<TMP_Text>();
        txt.text = Loc.T(es, string.IsNullOrEmpty(en) ? es : en);
    }
}