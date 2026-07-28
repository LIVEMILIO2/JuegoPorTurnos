using UnityEngine;

public class TileEspecial : MonoBehaviour
{
    [Header("Efecto")]
    [Tooltip("Positivo = sube iniciativa, Negativo = baja iniciativa")]
    public int efectoIniciativa = 10;

    [Header("Visual")]
    public Color colorBuff = new Color(0f, 1f, 0.4f, 1f);   // Verde brillante
    public Color colorDebuff = new Color(1f, 0.2f, 0.2f, 1f); // Rojo brillante

    void Start()
    {
        // Pintar el tile según el tipo de efecto
        var rend = GetComponent<Renderer>();
        if (rend != null)
            rend.material.color = efectoIniciativa >= 0 ? colorBuff : colorDebuff;
    }

    /// <summary>
    /// Aplica el efecto al PlayerScript si está parado encima.
    /// </summary>
    public void AplicarEfecto(PlayerScript player)
    {
        if (player == null) return;
        GameManager.Instance.ModificarIniciativa(player, player.iniciativa + efectoIniciativa);
        GameManager.Instance.ReconstruirColaActual();

        string tipo = efectoIniciativa >= 0 ? "gana" : "pierde";
        Debug.Log($"{player.name} {tipo} {Mathf.Abs(efectoIniciativa)} de iniciativa por tile especial. Iniciativa: {player.iniciativa}");
    }

    /// <summary>
    /// Aplica el efecto al EnemyScript si está parado encima.
    /// </summary>
    public void AplicarEfecto(EnemyScript enemy)
    {
        if (enemy == null) return;
        GameManager.Instance.ModificarIniciativa(enemy, enemy.iniciativa + efectoIniciativa);
        GameManager.Instance.ReconstruirColaActual();

        string tipo = efectoIniciativa >= 0 ? "gana" : "pierde";
        Debug.Log($"{enemy.name} {tipo} {Mathf.Abs(efectoIniciativa)} de iniciativa por tile especial. Iniciativa: {enemy.iniciativa}");
    }
}