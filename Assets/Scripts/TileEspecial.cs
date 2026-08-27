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

    public void AplicarEfecto(PlayerScript player)
    {
        if (player == null) return;
        // Revertir efecto anterior
        GameManager.Instance.ModificarIniciativa(player, player.iniciativa - player.efectoTileActual);
        // Aplicar nuevo efecto
        player.efectoTileActual = efectoIniciativa;
        GameManager.Instance.ModificarIniciativa(player, player.iniciativa + efectoIniciativa);
        GameManager.Instance.ReconstruirColaActual();
        Debug.Log($"{player.name} tile especial: iniciativa ahora {player.iniciativa}");
    }

    public void AplicarEfecto(EnemyScript enemy)
    {
        if (enemy == null) return;
        GameManager.Instance.ModificarIniciativa(enemy, enemy.iniciativa - enemy.efectoTileActual);
        enemy.efectoTileActual = efectoIniciativa;
        GameManager.Instance.ModificarIniciativa(enemy, enemy.iniciativa + efectoIniciativa);
        GameManager.Instance.ReconstruirColaActual();
        Debug.Log($"{enemy.name} tile especial: iniciativa ahora {enemy.iniciativa}");
    }
}