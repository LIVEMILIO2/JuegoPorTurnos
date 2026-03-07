using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Juego");
    }


    public void Salir()
    {
        Application.Quit();
        Debug.Log("Salir del juego");
    }
}