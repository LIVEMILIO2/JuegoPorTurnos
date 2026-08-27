using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0.5f;
    public float damage = 50f;
    public float health = 100f;
    [HideInInspector] public float healthMax = 100f;
    public float rangoAtaque = 1.5f;
    public float rangoHabilidad = 2f;
    public int playerMoveRange = 3;
    public string playerStats = "Warrior";

    [Header("Iniciativa")]
    public int iniciativa = 10;

    [Header("Barra de turnos")]
    public Sprite portrait;

    [Header("Indicador de turno")]
    public GameObject indicadorTurno;

    [Header("Animaciones")]
    public Animator animator;
    public RuntimeAnimatorController walk;

    [Header("Rotacion")]
    public float rotationSpeed = 10f;

    [Header("Panel de acciones")]
    public ActionPanelBase panelAcciones;

    [HideInInspector] public bool yaSeMovio = false;
    [HideInInspector] public bool yaUsoAccion = false;
    [HideInInspector] public bool estaDefendiendo = false;
    [HideInInspector] public int efectoTileActual = 0;

    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moviendose = false;

    void Start()
    {
        healthMax = health;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (moviendose) Mover();
    }

    void Mover()
    {
        Vector3 posAnterior = transform.position;
        Vector3 target = path[index];

        Vector3 direccion = (target - transform.position).normalized;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, rotationSpeed * Time.deltaTime);
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        GraphCreator.Instance?.ActualizarTileUnidad(posAnterior, transform.position, true);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            index++;

            if (index >= path.Count)
            {
                moviendose = false;
                yaSeMovio = true;

                // Quitar efecto del tile anterior
                GameManager.Instance.QuitarEfectoTile(this);
                // Aplicar nuevo tile si hay
                GameManager.Instance.AplicarTileEspecialPlayer(this);

                panelAcciones?.RefrescarBotones(this);

                if (TutorialManager.Instance != null)
                    TutorialManager.Instance.VerificarMovimiento(this);
                else if (yaUsoAccion)
                    GameManager.Instance.SiguienteTurno();
            }
        }
    }

    public void SetPath(List<Vector3> nuevoPath)
    {
        if (yaSeMovio) return;
        path = nuevoPath;
        index = 0;
        if (path.Count == 0)
        {
            panelAcciones?.RefrescarBotones(this);
            return;
        }
        moviendose = true;
        if (animator != null) animator.runtimeAnimatorController = walk;
    }

    public void ReiniciarTurno()
    {
        yaSeMovio = false;
        yaUsoAccion = false;
        estaDefendiendo = false;
        if (indicadorTurno != null) indicadorTurno.SetActive(true);
        foreach (var p in GameManager.Instance.players)
            if (p != null && p != this && p.panelAcciones != null)
                p.panelAcciones.OcultarPanel();
        panelAcciones?.MostrarPanel(this);
    }

    public void OcultarPanel() => panelAcciones?.OcultarPanel();
    public bool EsMiTurno() => GameManager.Instance.JugadorActual() == this;
    public bool EstaMoviendose() => moviendose;

    public void RecibirDamage(float cantidad)
    {
        if (estaDefendiendo) cantidad *= 0.5f;
        health -= cantidad;
        Debug.Log($"{name} recibe daño: {cantidad}. Vida restante: {health}");
        if (health <= 0) Die();
    }

    void Die()
    {
        Debug.Log($"{name} ha muerto.");
        panelAcciones?.OcultarPanel();
        GameManager.Instance?.RemovePlayer(this);
        Destroy(gameObject);
    }
}