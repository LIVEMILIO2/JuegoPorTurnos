using UnityEngine;

public abstract class ActionPanelBase : MonoBehaviour
{
    public abstract void MostrarPanel(PlayerScript player);
    public abstract void OcultarPanel();
    public abstract void RefrescarBotones(PlayerScript player);

    // Llamado por TutorialManager para bloquear/desbloquear botones según el paso
    public virtual void ModoTutorial(TutorialManager.AccionEsperada accion, string personaje)
    {
        // Implementación base vacía — cada panel la sobreescribe
    }
}