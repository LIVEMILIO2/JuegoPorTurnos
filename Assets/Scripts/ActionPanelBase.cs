using UnityEngine;

// Clase base que reemplaza IActionPanel.
// Al ser MonoBehaviour, Unity puede serializarla correctamente en el Inspector.
public abstract class ActionPanelBase : MonoBehaviour
{
    public abstract void MostrarPanel(PlayerScript player);
    public abstract void OcultarPanel();
    public abstract void RefrescarBotones(PlayerScript player);
}