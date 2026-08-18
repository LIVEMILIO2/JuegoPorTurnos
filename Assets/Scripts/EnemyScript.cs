using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float speed = 5f;
    public float altura = 0f;
    public float Heatlh = 100;
    public float currentHealth;
    public int enemyMoveRange = 3;
    public float enemyAtackRange = 1.5f;
    public float damage = 25f;

    [Header("Iniciativa")]
    public int iniciativa = 5;

    [Header("Barra de turnos")]
    public Sprite portrait;

    [Header("Indicador de turno")]
    public GameObject indicadorTurno;

    [Header("Animaciones")]
    public Animator animator;
    public RuntimeAnimatorController walk;

    [Header("Rotacion")]
    public float rotationSpeed = 10f;

    List<Vector3> path = new List<Vector3>();
    int index = 0;
    bool moving = false;
    PlayerScript objetivoActual;

    void Start()
    {
        currentHealth = Heatlh;
    }

    void Update()
    {
        if (moving) Mover();
    }

    public void TomarTurno()
    {
        if (indicadorTurno != null) indicadorTurno.SetActive(true);

        objetivoActual = ObtenerObjetivo();
        if (objetivoActual == null)
        {
            GameManager.Instance.SiguienteTurno();
            return;
        }

        Vector2Int start = GraphCreator.Instance.WorldToGrid(transform.position);
        Vector2Int goal = GraphCreator.Instance.WorldToGrid(objetivoActual.transform.position);
        GraphCreator.Instance.CalcularCaminoEnemy(start, goal, this);
    }

    public void SetPath(List<Vector3> nuevoPath)
    {
        path = nuevoPath;
        index = 0;

        if (path.Count == 0)
        {
            IntentarAtacar();
            GameManager.Instance.SiguienteTurno();
            return;
        }

        moving = true;
        if (animator != null) animator.runtimeAnimatorController = walk;
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
        GraphCreator.Instance?.ActualizarTileUnidad(posAnterior, transform.position, false);

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
            index++;

            if (index >= path.Count)
            {
                moving = false;

                // Aplicar tile especial al llegar al destino
                GameManager.Instance.AplicarTileEspecialEnemy(this);

                IntentarAtacar();
                GameManager.Instance.SiguienteTurno();
            }
        }
    }

    void IntentarAtacar()
    {
        if (objetivoActual == null) return;
        float distancia = Vector3.Distance(transform.position, objetivoActual.transform.position);
        if (distancia <= enemyAtackRange)
            objetivoActual.RecibirDamage(damage);
    }

    PlayerScript ObtenerObjetivo()
    {
        PlayerScript objetivo = null;
        float mejorDistancia = Mathf.Infinity;
        foreach (var player in GameManager.Instance.players)
        {
            float d = Vector3.Distance(transform.position, player.transform.position);
            if (d < mejorDistancia) { mejorDistancia = d; objetivo = player; }
        }
        return objetivo;
    }

    public void RecibirDamage(float cantidad)
    {
        currentHealth -= cantidad;
        Debug.Log("Enemy vida: " + currentHealth);
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        GameManager.Instance?.RemoveEnemy(this);
        Destroy(gameObject);
    }

    public bool Moving() => moving;
}