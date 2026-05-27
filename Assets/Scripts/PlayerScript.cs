using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0f;
    public float damage = 50f;
    public float health = 100f;
    public float rangoAtaque = 1.5f;
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
    public float rotationSpeed = 10f; // Que tan rapido gira

    [HideInInspector] public bool yaSeMovio = false;
    [HideInInspector] public bool yaUsoAccion = false;
    [HideInInspector] public bool estaDefendiendo = false;

    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moviendose = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (moviendose) Mover();
    }

    void Mover()
    {
        Vector3 target = path[index];

        // Rotar hacia el destino antes de moverse
        Vector3 direccion = (target - transform.position).normalized;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                rotationSpeed * Time.deltaTime
            );
        }

        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            index++;

            if (index >= path.Count)
            {
                moviendose = false;
                yaSeMovio = true;
                ActionPanelUI.Instance?.RefrescarBotones(this);

                if (yaUsoAccion)
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
            ActionPanelUI.Instance?.RefrescarBotones(this);
            return;
        }
        moviendose = true;
        animator.runtimeAnimatorController = walk;
    }

    public void ReiniciarTurno()
    {
        yaSeMovio = false;
        yaUsoAccion = false;
        estaDefendiendo = false;
        if (indicadorTurno != null) indicadorTurno.SetActive(true);
        ActionPanelUI.Instance?.MostrarPanel(this);
    }

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
        ActionPanelUI.Instance?.OcultarPanel();
        GameManager.Instance?.RemovePlayer(this);
        Destroy(gameObject);
    }
}