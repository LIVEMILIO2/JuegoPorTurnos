using UnityEngine;

public class botonTurno : MonoBehaviour
{
    GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    //  ESTA ES LA FUNCION DEL BOTON
    public void BotonPasarTurno()
    {
        Debug.Log("BOTON PASAR TURNO PRESIONADO");

        gameManager.SiguienteTurno();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
