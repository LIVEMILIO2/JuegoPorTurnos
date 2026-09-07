using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnSlot : MonoBehaviour
{
    [Header("Referencias")]
    public Image fondo;
    public Image borde;
    public Texture2D portrait;
    public TMP_Text label;

    [Header("Colores")]
    public Color colorPlayer = new Color(0.2f, 0.5f, 1f);
    public Color colorEnemy = new Color(0.9f, 0.2f, 0.2f);
    public Color colorBorde = Color.yellow;

    public void Setup(bool esPlayer, bool esActivo, Texture2D texture)
    {
        fondo.color = esPlayer ? colorPlayer : colorEnemy;

        if (texture != null)
        {
            fondo.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            if (label != null) label.gameObject.SetActive(false);
        }
        else if (label != null)
        {
            label.gameObject.SetActive(true);
            label.text = esPlayer ? "P" : "E";
        }

        if (borde != null)
        {
            borde.gameObject.SetActive(esActivo);
            borde.color = colorBorde;
        }

        transform.localScale = esActivo ? Vector3.one * 1.15f : Vector3.one;
    }
}