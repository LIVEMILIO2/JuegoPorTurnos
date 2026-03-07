using UnityEngine;

public class botonTurno : MonoBehaviour
{
    GameManager gameManager;
    void Start()
    {
        
    }

    public void BotonPasarTurno()
    {
        Debug.Log("BOTON PASAR TURNO PRESIONADO");

        gameManager.SiguienteTurno();
    }

    void Update()
    {
        
    }
}
