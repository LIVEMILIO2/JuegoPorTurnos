using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("UI Tutorial")]
    public GameObject panelTutorial;
    public TMP_Text textoInstruccion;
    public Button btnSiguiente;

    [Header("Escena siguiente")]
    public string nombreEscenaJuego = "Level1";

    private int pasoActual = 0;
    private Vector3 posicionInicialJugador;

    public enum AccionEsperada
    {
        Ninguna,
        Moverse,
        Atacar,
        Habilidad1,
        Habilidad2,
    }

    private AccionEsperada accionEsperada = AccionEsperada.Ninguna;

    private struct Paso
    {
        public string texto;
        public AccionEsperada accion;
        public string personaje;
    }

    private Paso[] pasos;

    void Awake()
    {
        Instance = this;
    }
    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
    void Start()
    {
        pasos = new Paso[]
        {
            new Paso { texto = "¡Bienvenido! Este es un juego de estrategia por turnos.\nCada personaje actúa según su iniciativa.", accion = AccionEsperada.Ninguna, personaje = "" },
            new Paso { texto = "El orden de turnos depende de la iniciativa de cada personaje.\nQuien tenga más iniciativa actúa primero.", accion = AccionEsperada.Ninguna, personaje = "" },
            new Paso { texto = "Es el turno del Warrior.\nPresiona MOVER y muévete cerca del enemigo.", accion = AccionEsperada.Moverse, personaje = "Warrior" },
            new Paso { texto = "¡Bien! Ahora ataca al enemigo.\nPresiona ATACAR y haz clic en el enemigo.", accion = AccionEsperada.Atacar, personaje = "Warrior" },
            new Paso { texto = "El Warrior puede usar ACELERAR para subir su iniciativa y actuar antes.", accion = AccionEsperada.Habilidad1, personaje = "Warrior" },
            new Paso { texto = "También puede usar DEFENDER para reducir a la mitad el daño que recibe.", accion = AccionEsperada.Habilidad2, personaje = "Warrior" },
            new Paso { texto = "Es el turno del Mage.\nPresiona MOVER y muévete cerca del enemigo.", accion = AccionEsperada.Moverse, personaje = "Mage" },
           // new Paso { texto = "¡Bien! Ahora ataca al enemigo.\nPresiona ATACAR y haz clic en el enemigo.", accion = AccionEsperada.Atacar, personaje = "Mage" },
            new Paso { texto = "El Mage puede usar STUN para bajar la iniciativa de un enemigo.", accion = AccionEsperada.Habilidad1, personaje = "Mage" },
            new Paso { texto = "Es el turno del Support.\nEl Support puede CURAR a un aliado cercano.", accion = AccionEsperada.Habilidad1, personaje = "Support" },
            new Paso { texto = "El Support también puede usar BUFF para subir la iniciativa de un aliado.", accion = AccionEsperada.Habilidad2, personaje = "Support" },
            new Paso { texto = "¡Perfecto! Ya conoces todas las mecánicas del juego.\n¡Buena suerte en la batalla!", accion = AccionEsperada.Ninguna, personaje = "" },
        };

        MostrarPaso(0);
    }

    void MostrarPaso(int indice)
    {
        pasoActual = indice;
        Paso paso = pasos[indice];
        accionEsperada = paso.accion;

        textoInstruccion.text = paso.texto;
        btnSiguiente.gameObject.SetActive(paso.accion == AccionEsperada.Ninguna);

        if (paso.accion == AccionEsperada.Habilidad1 || paso.accion == AccionEsperada.Habilidad2)
        {
            PlayerScript jugador = ObtenerJugadorDePaso(paso.personaje);
            if (jugador != null) jugador.yaUsoAccion = false;
        }

        if (paso.accion == AccionEsperada.Moverse)
        {
            PlayerScript jugador = ObtenerJugadorDePaso(paso.personaje);
            if (jugador != null)
                posicionInicialJugador = jugador.transform.position;
        }

        NotificarPaneles(paso);
    }

    void NotificarPaneles(Paso paso)
    {
        var paneles = FindObjectsByType<ActionPanelBase>(FindObjectsSortMode.None);
        foreach (var panel in paneles)
            panel.ModoTutorial(paso.accion, paso.personaje);
    }

    public void NotificarCambioTurno()
    {
        NotificarPaneles(pasos[pasoActual]);
    }

    public void OnSiguiente()
    {
        AvanzarPaso();
    }

    public void AccionCompletada(AccionEsperada accion)
    {
        if (accion != accionEsperada) return;
        AvanzarPaso();
    }

    public void VerificarMovimiento(PlayerScript jugador)
    {
        Paso paso = pasos[pasoActual];

        float rangoNecesario = jugador.rangoAtaque + 1;

        if (pasoActual + 1 < pasos.Length)
        {
            AccionEsperada siguienteAccion = pasos[pasoActual + 1].accion;
            if (siguienteAccion == AccionEsperada.Habilidad1 || siguienteAccion == AccionEsperada.Habilidad2)
                rangoNecesario = jugador.rangoHabilidad;
        }

        bool hayTargetEnRango = false;

        foreach (var e in GameManager.Instance.enemies)
        {
            if (e == null) continue;
            if (Vector3.Distance(jugador.transform.position, e.transform.position) <= rangoNecesario)
            {
                hayTargetEnRango = true;
                break;
            }
        }

        if (!hayTargetEnRango && paso.personaje == "Support")
        {
            foreach (var p in GameManager.Instance.players)
            {
                if (p == null || p == jugador) continue;
                if (Vector3.Distance(jugador.transform.position, p.transform.position) <= rangoNecesario)
                {
                    hayTargetEnRango = true;
                    break;
                }
            }
        }

        if (!hayTargetEnRango)
        {
            jugador.transform.position = posicionInicialJugador;
            jugador.yaSeMovio = false;
            textoInstruccion.text = "¡Demasiado lejos! Muévete más cerca del objetivo e intenta de nuevo.";
            NotificarPaneles(pasos[pasoActual]);
        }
        else
        {
            AccionCompletada(AccionEsperada.Moverse);
        }
    }

    void AvanzarPaso()
    {
        int siguiente = pasoActual + 1;
        if (siguiente >= pasos.Length)
        {
            SceneManager.LoadScene(nombreEscenaJuego);
            return;
        }
        MostrarPaso(siguiente);
    }

    PlayerScript ObtenerJugadorDePaso(string personaje)
    {
        foreach (var p in GameManager.Instance.players)
            if (p != null && p.playerStats == personaje) return p;
        return null;
    }

    public bool EsUltimoPasoDelPersonaje()
    {
        string personajeActual = pasos[pasoActual].personaje;
        int siguiente = pasoActual + 1;
        if (siguiente >= pasos.Length) return true;
        return pasos[siguiente].personaje != personajeActual;
    }

    public AccionEsperada GetAccionEsperada() => accionEsperada;
    public string GetPersonajeEsperado() => pasos[pasoActual].personaje;
}