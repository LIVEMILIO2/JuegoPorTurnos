using UnityEngine;

public class botonTurno : MonoBehaviour
{
    public void BotonPasarTurno()
    {
        Debug.Log("BOTON PASAR TURNO PRESIONADO");
        GameManager.Instance.BotonPasarTurno();
    }
}